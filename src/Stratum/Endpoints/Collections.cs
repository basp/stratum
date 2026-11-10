namespace Stratum.Endpoints;

internal static class Collections
{
    public static IResult Get(
        LinkGenerator link,
        HttpContext context)
    {
        Link[] links =
        [
            new(
                link.GetPathByName(context, EndpointName.Collections)!,
                LinkRelationType.Self),
        ];

        CollectionDescription[] collections = 
        [
            new("test", 
            [
                new(
                    "/collections/test",
                    LinkRelationType.Data),
            ]),
        ];

        var response = new CollectionsResponse(
            links,
            collections);

        return TypedResults.Ok(response);
    }
}

internal sealed record CollectionsResponse(
    Link[] Links,
    CollectionDescription[] Collections);
    
internal sealed record CollectionDescription(
    string Id,
    Link[] Links);
    