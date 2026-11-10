namespace Stratum.Endpoints;

internal static class Conformance
{
    public static IResult Get()
    {
        return TypedResults.Ok();
    }
}