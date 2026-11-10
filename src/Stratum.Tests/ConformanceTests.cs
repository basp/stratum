using System.Net;
using System.Text.Json;

namespace Stratum.Tests;

public sealed class ConformanceTests(StratumApiFactory factory)
    : IClassFixture<StratumApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Req_Core_ConformanceOp()
    {
        var response = await this.client.GetAsync("/conformance");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Req_Core_ConformanceSuccess()
    {
        var response = await this.client.GetAsync("/conformance");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var conformsTo = json
            .GetProperty("conformsTo")
            .EnumerateArray()
            .Select(el => el.GetString())
            .ToArray();

        Assert.Contains(
            "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core",
            conformsTo);
        Assert.Contains(
            "http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30",
            conformsTo);
    }
}
