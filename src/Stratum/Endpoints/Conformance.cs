namespace Stratum.Endpoints;

internal static class Conformance
{
    public static IResult Get()
    {
        var response = new ConformanceResponse(
        [
            WellKnownConstants.CoreConformance,
            WellKnownConstants.Oas3Conformance,
        ]);
        return TypedResults.Ok(response);
    }
}

internal sealed record ConformanceResponse(IReadOnlyList<string> ConformsTo);
