var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app
    .MapGet(
        "/",
        () =>
        {
            object[] links =
            [
                new
                {
                    href = "/conformance",
                    rel = LinkRelationType.Conformance,
                },
                new
                {
                    href ="/collections",
                    rel = LinkRelationType.Data,
                },
                new
                {
                    href = "/api.html",
                    rel = LinkRelationType.ServiceDocumentation,
                },
                new
                {
                    href = "/api",
                    rel = LinkRelationType.ServiceDescription,
                },
                new
                {
                    href = "/",
                    rel = LinkRelationType.Self,
                }
            ];

            return new
            { 
                links
            };
        })
    .WithName(EndpointName.LandingPage);

app
    .MapGet(
        "/conformance", 
        () => Results.Ok())
    .WithName(EndpointName.Conformance);

app
    .MapGet(
        "/collections",
        () => Results.Ok())
    .WithName(EndpointName.Collections);

app.Run();

internal static class EndpointName
{
    public const string LandingPage = "landingPage";
    public const string Conformance = "conformance";
    public const string Collections = "collections";
}

internal static class LinkRelationType
{
    public const string Alternate = "alternate";
    public const string Collection = "collection";
    public const string Conformance = "conformance";
    public const string Data = "data";
    public const string DescribedBy = "describedby";
    public const string Item = "item";
    public const string Items = "items";
    public const string Next = "next";
    public const string License = "license";
    public const string Prev = "prev";
    public const string Self = "self";
    public const string ServiceDescription = "service-desc";
    public const string ServiceDocumentation = "service-doc";
}