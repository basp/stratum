namespace Stratum.Endpoints;

internal static class Collections
{
    private const string ApplicationType = "application/geo+json";
    private const string Title = "items";
    
    public static IResult Get(
        LinkGenerator links,
        HttpContext context)
    {
        Link[] endpointLinks =
        [
            new(
                links.GetPathByName(context, EndpointName.Collections)!,
                LinkRelationType.Self),
        ];

        CollectionDescription[] collections = 
        [
            new("test",
                // Fake set of links, still need to implement these.
                // Here, "test" is a collection identifier.
            [
                new(
                    "/collections/test",
                    LinkRelationType.Self),
                new(
                    "/collections/test/items",
                    LinkRelationType.Items,
                    Type: Collections.ApplicationType,
                    Title: Title),
            ]),
        ];

        var response = new CollectionsResponse(
            endpointLinks,
            collections);

        return TypedResults.Ok(response);
    }
}

internal sealed record CollectionsResponse(
    IReadOnlyList<Link> Links,
    IReadOnlyList<CollectionDescription> Collections);

internal sealed record CollectionDescription(
    string Id,
    IReadOnlyList<Link> Links);
    