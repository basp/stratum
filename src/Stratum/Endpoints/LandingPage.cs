namespace Stratum.Endpoints;

public static class LandingPage
{
    public static IResult Get(LinkGenerator links, HttpContext context) =>
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
                links.GetPathByName(context, EndpointName.ServiceDocumentation)!,
                LinkRelationType.ServiceDocumentation),
            new(
                links.GetPathByName(context, EndpointName.ServiceDescription)!,
                LinkRelationType.ServiceDescription),
        ]));
}

internal sealed record Link(
    string Href, 
    string Rel);

internal sealed record LandingPageResponse(
    IReadOnlyList<Link> Links);