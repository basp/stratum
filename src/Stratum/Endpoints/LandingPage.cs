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
                Type: WellKnownConstants.OpenApiType,
                Title: WellKnownConstants.OpenApiTitle),
        ]));
}

internal sealed record LandingPageResponse(
    IReadOnlyList<Link> Links);
    