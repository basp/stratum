namespace Stratum.Endpoints;

internal static class Collections
{
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
            [
                new(
                    "/collections/test",
                    LinkRelationType.Self),
                new(
                    "/collections/test/items",
                    LinkRelationType.Items,
                    Type: "application/geo+json",
                    Title: "items"),
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
    