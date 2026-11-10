using System.Net;
using System.Text.Json;

namespace Stratum.Tests;

public sealed class LandingPageTests(StratumApiFactory factory)
    : IClassFixture<StratumApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Req_Core_RootOp()
    {
        _ = await this.client.GetAsync("/");
    }
    
    [Fact]
    public async Task Req_Core_RootSuccess()
    {
        var response = await this.client.GetAsync("/");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var links = json
            .GetProperty("links")
            .EnumerateArray()
            .ToArray();
        Assert.Contains(links, el =>
            el.GetProperty("rel").GetString() == "service-desc");
        Assert.Contains(links, el =>
            el.GetProperty("href").GetString() == "/conformance" &&
            el.GetProperty("rel").GetString() == "conformance");
        Assert.Contains(links, el =>
            el.GetProperty("href").GetString() == "/collections" &&
            el.GetProperty("rel").GetString() == "data");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Rec_Core_RootLinks()
    {
        var response = await this.client.GetAsync("/");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var links = json
            .GetProperty("links")
            .EnumerateArray()
            .ToArray();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(links, el =>
            el.GetProperty("rel").GetString() == "self" &&
            el.GetProperty("href").GetString() == "/");
    }
}
 
