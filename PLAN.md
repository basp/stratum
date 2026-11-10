# Stratum Development Plan & Roadmap

### Project Vision & Roadmap
Stratum is a high-performance, specification-compliant implementation of the **OGC API - Features (Part 1: Core)** standard built with ASP.NET Core 9 Minimal APIs. The objective of this development plan is to maximize development throughput and deliver incremental utility by breaking down specification requirements into bite-sized, prioritized iterations.

Each phase builds directly upon the existing type-safe, immutable Minimal API architecture, ensuring automated regression testing and compliance with the requirement checklist in `README.md`.

---

### Phase 1: API Definition & Spec Verification

#### Objectives & Utility
Validate the existing OpenAPI 3.0 service description endpoint to check off outstanding core specification requirements with minimal development effort.

#### Specification Requirements
- `/req/core/api-definition-op`: The server SHALL provide an API definition at `/api`.
- `/req/core/api-definition-success`: A successful response to `GET /api` SHALL return status code `200 OK` and a valid OpenAPI 3.0 document.

#### Implementation Scope
1. **Integration Test Suite (`ApiDefinitionTests.cs` in `src/Stratum.Tests`):**
   - Implement `Req_Core_ApiDefinitionOp` verifying `GET /api` returns `HttpStatusCode.OK`.
   - Implement `Req_Core_ApiDefinitionSuccess` asserting that the response has a valid JSON content type and contains required OpenAPI fields (e.g., `openapi: "3.0.1"`, `info`, `paths`).
2. **README Checklist Update:**
   - Mark `/req/core/api-definition-op` and `/req/core/api-definition-success` as complete in `README.md`.

#### Acceptance Criteria
- [ ] Integration tests in `ApiDefinitionTests.cs` pass consistently.
- [ ] The OpenAPI output is confirmed to document all active endpoints (`/`, `/conformance`, `/collections`).
- [ ] Requirements checklist in `README.md` is updated.

---

### Phase 2: Collection Metadata & Extent Models

#### Objectives & Utility
Elevate collection representations to full standard compliance by introducing strongly typed spatial/temporal extent models, implementing the single collection metadata endpoint (`GET /collections/{collectionId}`), and replacing hardcoded collection paths with dynamic route generation.

#### Specification Requirements
- `/req/core/fc-md-extent`: Each collection SHALL provide an `extent` object with spatial bounding box and temporal interval definitions.
- `/req/core/fc-md-links`: The links array for each collection in the collection list SHALL include relations to `self` and `items`.
- `/req/core/sfc-md-op`: The server SHALL support `GET /collections/{collectionId}`.
- `/req/core/sfc-md-success`: A successful response SHALL return metadata for the requested single collection.

#### Implementation Scope
1. **Domain Models for Extents (`src/Stratum/Endpoints/Extent.cs`):**
   - Create immutable record structures using `IReadOnlyList<T>`:
     ```csharp
     internal sealed record Extent(
         SpatialExtent? Spatial = null,
         TemporalExtent? Temporal = null);

     internal sealed record SpatialExtent(
         IReadOnlyList<IReadOnlyList<double>> Bbox,
         string? Crs = null);

     internal sealed record TemporalExtent(
         IReadOnlyList<IReadOnlyList<string?>> Interval,
         string? Trs = null);
     ```
   - Leverage global `JsonIgnoreCondition.WhenWritingNull` so optional elements (`Crs`, `Trs`) omit cleanly when unspecified.

2. **Single Collection Endpoint (`GET /collections/{collectionId}`):**
   - Add `EndpointName.Collection = "Collection"` and `EndpointName.CollectionItems = "CollectionItems"` to `EndpointName.cs`.
   - Implement `src/Stratum/Endpoints/Collection.cs` mapped to route `/collections/{collectionId}`.
   - Return `TypedResults.Ok(CollectionDescription)` or `TypedResults.NotFound()` when an unknown ID is queried.

3. **Dynamic Route Generation in `Collections.cs` and `Collection.cs`:**
   - Use `links.GetPathByName(context, EndpointName.Collection, new { collectionId = id })` for `rel: "self"`.
   - Use `links.GetPathByName(context, EndpointName.CollectionItems, new { collectionId = id })` for `rel: "items"`.

4. **Integration Testing:**
   - Create `SingleCollectionTests.cs` verifying `GET /collections/{collectionId}` returns status `200 OK` for existing collections and `404 NotFound` for invalid IDs.
   - Validate serialization of `extent.spatial.bbox` and `extent.temporal.interval`.

#### Acceptance Criteria
- [ ] `CollectionDescription` includes an `Extent` property serialized according to OGC JSON schema rules.
- [ ] `GET /collections/{collectionId}` returns typed `200 OK` or `404 Not Found`.
- [ ] All collection links are generated dynamically without hardcoded path strings.
- [ ] Requirements `/req/core/fc-md-extent`, `/req/core/fc-md-links`, `/req/core/sfc-md-op`, and `/req/core/sfc-md-success` pass in test suite and are marked in `README.md`.

---

### Phase 3: Spatial Database & Feature Items Endpoint

#### Objectives & Utility
Establish database-backed spatial persistence using PostgreSQL/PostGIS with Entity Framework Core and NetTopologySuite to serve GeoJSON FeatureCollections from `GET /collections/{collectionId}/items`.

#### Specification Requirements
- `/req/core/fc-op`: The server SHALL support `GET /collections/{collectionId}/items`.
- `/req/core/fc-response`: The response SHALL be a GeoJSON FeatureCollection with `type: "FeatureCollection"`, `features`, and pagination/self links.
- `/req/core/fc-md-items`: The collection description SHALL link to the items endpoint with relation `items`.
- `/req/core/fc-md-items-links`: Each item collection response SHALL contain `self` and `alternate` links.
- `/req/core/sfc-op`: The server SHALL support `GET /collections/{collectionId}/items/{featureId}`.
- `/req/core/sfc-response`: The response SHALL be a single GeoJSON Feature object.

#### Implementation Scope
1. **Spatial Data Layer Configuration:**
   - Add package reference `Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite` to `Stratum.csproj`.
   - Configure EF Core DbContext to enable NetTopologySuite:
     ```csharp
     options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
     ```
   - Define entities representing feature collections and spatial feature items with `NetTopologySuite.Geometries.Geometry`.

2. **GeoJSON Response Models (`src/Stratum/Endpoints/Features/`):**
   - Create typed records for `FeatureCollectionResponse`, `FeatureResponse`, and feature properties.
   - Ensure coordinate arrays and geometry types map to RFC 7946 GeoJSON format.

3. **Feature Endpoints Implementation:**
   - `GET /collections/{collectionId}/items` (`EndpointName.CollectionItems`): Queries features for the collection and returns `FeatureCollectionResponse`.
   - `GET /collections/{collectionId}/items/{featureId}` (`EndpointName.FeatureItem`): Queries a specific feature by ID and returns `FeatureResponse` or `404 Not Found`.

4. **Integration Testing:**
   - Implement `FeatureItemsTests.cs` validating `FeatureCollection` schema, geometry output, and HTTP status codes (`200 OK`, `404 NotFound`).

#### Acceptance Criteria
- [ ] Database migrations configure PostGIS and spatial index structures.
- [ ] `GET /collections/{collectionId}/items` returns valid GeoJSON `FeatureCollection` payloads with `type: "application/geo+json"`.
- [ ] `GET /collections/{collectionId}/items/{featureId}` returns individual GeoJSON `Feature` representations.
- [ ] Requirements `/req/core/fc-op`, `/req/core/fc-response`, `/req/core/sfc-op`, and `/req/core/sfc-response` pass in integration test suite.

---

### Phase 4: Query Parameters, Filtering & Content Negotiation

#### Objectives & Utility
Implement standard query parameters (`limit`, `bbox`, `datetime`) for feature retrieval, support spatial filtering against PostGIS bounding boxes, and introduce format negotiation via query parameters (`?f=json`, `?f=html`) and HTTP `Accept` headers.

#### Specification Requirements
- `/req/core/rc-bbox-definition`: The server SHALL support the `bbox` query parameter (4 or 6 coordinates).
- `/req/core/rc-bbox-response`: Only features intersecting the requested bounding box SHALL be returned.
- `/req/core/rc-limit-definition`: The server SHALL support the `limit` query parameter with server-enforced min/max/default values.
- `/req/core/rc-limit-response`: The number of returned features SHALL NOT exceed the requested limit.
- `/req/core/rc-datetime-definition`: The server SHALL support date-time point and interval filtering.
- `/req/core/query-param-unknown`: The server SHALL return status code `400 Bad Request` or ignore unknown parameters per spec profile.
- `/req/core/query-param-invalid`: The server SHALL return status code `400 Bad Request` when query parameter values cannot be parsed.
- `/req/core/crs84`: Coordinates SHALL be in WGS 84 (longitude, latitude) unless an alternative CRS is negotiated.
- `/req/core/http` & `/rec/core/html`: The server SHALL support content negotiation via `Accept` headers and `?f=` format parameter.

#### Implementation Scope
1. **Query Filter Binding & Validation:**
   - Implement strongly typed query parameter record `FeatureQueryParameters(double[]? bbox, string? datetime, int? limit, int? offset, string? f)`.
   - Validate `bbox` coordinate counts (must be 4 or 6 doubles) and range limits (`-180 <= lon <= 180`, `-90 <= lat <= 90`).
   - Validate ISO 8601 date-time strings and intervals (`start/end`, `../end`, `start/..`).
   - Clamp `limit` to range `1..1000` (default: 10).

2. **Spatial Query Execution (LINQ + NetTopologySuite):**
   - Translate valid `bbox` parameter into NetTopologySuite `Envelope` / `Polygon`.
   - Apply `.Where(f => f.Geometry.Intersects(queryEnvelope))` with PostGIS bounding-box operators (`&&`) for index acceleration.
   - Generate `next` and `prev` pagination links based on limit/offset or cursor markers.

3. **Content Negotiation Middleware/Endpoint Filters:**
   - Map `?f=json` and `Accept: application/geo+json` / `application/json` to GeoJSON responses.
   - Add HTML representation endpoints (`?f=html` / `Accept: text/html`) with Razor/Liquid or minimal HTML templates.

4. **Integration Testing:**
   - Create `QueryFilterTests.cs` verifying spatial bbox intersections, date-time ranges, limit boundaries, and `400 Bad Request` on invalid parameter inputs.

#### Acceptance Criteria
- [ ] Bounding box (`bbox`) spatial filtering accurately constrains returned features.
- [ ] Date-time filtering returns only features matching the specified temporal instant or interval.
- [ ] Paging `limit` and pagination links (`next`/`prev`) navigate large datasets reliably.
- [ ] Invalid query parameters return `400 Bad Request` with structured error descriptions.
- [ ] Format negotiation supports JSON/GeoJSON and HTML representations.

---

### Specification Traceability Matrix

| Requirement / Recommendation Code | Description | Roadmap Phase |
| :--- | :--- | :--- |
| `/req/core/root-op` | Root endpoint exists at `/` | Complete |
| `/req/core/root-success` | Root response status `200 OK` and valid landing page | Complete |
| `/req/core/conformance-op` | Conformance endpoint exists at `/conformance` | Complete |
| `/req/core/conformance-success` | Returns `conformsTo` list including Core URI | Complete |
| `/req/core/api-definition-op` | API definition endpoint exists at `/api` | **Phase 1** |
| `/req/core/api-definition-success` | Returns OpenAPI 3.0 document with `200 OK` | **Phase 1** |
| `/req/core/fc-md-op` | Collection list endpoint exists at `/collections` | Complete |
| `/req/core/fc-md-success` | Returns collection descriptions array | Complete |
| `/req/core/fc-md-extent` | Collection metadata includes spatial/temporal extents | **Phase 2** |
| `/req/core/fc-md-links` | Collection list items link to `self` and `items` | **Phase 2** |
| `/req/core/sfc-md-op` | Single collection endpoint exists at `/collections/{collectionId}` | **Phase 2** |
| `/req/core/sfc-md-success` | Returns single collection metadata with `200 OK` | **Phase 2** |
| `/req/core/fc-op` | Feature items endpoint at `/collections/{collectionId}/items` | **Phase 3** |
| `/req/core/fc-response` | Items endpoint returns GeoJSON FeatureCollection | **Phase 3** |
| `/req/core/fc-md-items` | Collection description links to items endpoint | **Phase 3** |
| `/req/core/fc-md-items-links` | Item collection responses contain self and navigation links | **Phase 3** |
| `/req/core/sfc-op` | Single feature item endpoint at `/collections/{id}/items/{featureId}` | **Phase 3** |
| `/req/core/sfc-response` | Single feature response returns GeoJSON Feature | **Phase 3** |
| `/req/core/rc-bbox-definition` | Support `bbox` query parameter | **Phase 4** |
| `/req/core/rc-bbox-response` | Filter features by spatial intersection with `bbox` | **Phase 4** |
| `/req/core/rc-limit-definition` | Support `limit` query parameter with bounds | **Phase 4** |
| `/req/core/rc-limit-response` | Enforce maximum feature count in response | **Phase 4** |
| `/req/core/rc-datetime-definition` | Support `datetime` query parameter for temporal filtering | **Phase 4** |
| `/req/core/rc-datetime-response` | Filter features by temporal intersection with `datetime` | **Phase 4** |
| `/req/core/query-param-unknown` | Reject or handle unrecognized query parameters | **Phase 4** |
| `/req/core/query-param-invalid` | Return `400 Bad Request` on unparseable query parameters | **Phase 4** |
| `/req/core/crs84` | Default coordinate representation is WGS 84 (CRS84) | **Phase 4** |
| `/req/core/http` | Standard HTTP content negotiation via headers and parameters | **Phase 4** |
| `/rec/core/root-links` | Landing page links to metadata and data | Complete |
| `/rec/core/api-definition-oas` | Media type and title on API definition links | Complete |
| `/rec/core/html` | HTML representation of landing page, conformance, and features | **Phase 4** |
