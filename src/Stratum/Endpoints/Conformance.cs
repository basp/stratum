namespace Stratum.Endpoints;

internal static class Conformance
{
    public static IResult Get()
    {
        var response = new ConformanceResponse(
        [
            "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core",
            "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30",
        ]);
        return TypedResults.Ok(response);
    }
}

internal sealed record ConformanceResponse(IReadOnlyList<string> ConformsTo);
