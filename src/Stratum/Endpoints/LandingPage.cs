namespace Stratum.Endpoints;

public static class LandingPage
{
    public static IResult Get() =>
        TypedResults.Ok(new LandingPageResponse(
        [
            new("/conformance", LinkRelationType.Conformance),
            new("/collections", LinkRelationType.Data),
            new("/api.html", LinkRelationType.ServiceDocumentation),
            new("/api", LinkRelationType.ServiceDescription),
            new("/", LinkRelationType.Self),
        ]));
}

public sealed record Link(
    string Href, 
    string Rel);

public sealed record LandingPageResponse(
    IReadOnlyList<Link> Links);