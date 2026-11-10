namespace Stratum.Endpoints;

internal static class LandingPage
{
    public static IResult Get(
        LinkGenerator links, 
        HttpContext context) =>
        TypedResults.Ok(new LandingPageResponse(
        [
            new(
                links.GetPathByName(context, EndpointName.LandingPage)!,
                LinkRelationType.Self),
            new(
                links.GetPathByName(context, EndpointName.Conformance)!,
                LinkRelationType.Conformance),
            new(
                links.GetPathByName(context, EndpointName.Collections)!,
                LinkRelationType.Data),
            new(
                links.GetPathByName(context, EndpointName.ServiceDescription)!,
                LinkRelationType.ServiceDescription,
                Type: "application/vnd.oai.openapi+json;version=3.0",
                Title: "OpenAPI 3.0 specification"),
        ]));
}

internal sealed record LandingPageResponse(
    IReadOnlyList<Link> Links);
    