using System.Net;
using System.Text.Json;

namespace Stratum.Tests;

public sealed class CollectionsTests(StratumApiFactory factory)
    : IClassFixture<StratumApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Req_Core_FcMdOp()
    {
        var response = await this.client.GetAsync("/collections");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Req_Core_FcMdSuccess()
    {
        var response = await this.client.GetAsync("/collections");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var links = json
            .GetProperty("links")
            .EnumerateArray()
            .ToArray();
        Assert.Contains(links, el =>
            el.GetProperty("href").GetString() == "/collections" &&
            el.GetProperty("rel").GetString() == "self");

        var collections = json
            .GetProperty("collections")
            .EnumerateArray()
            .ToArray();
        Assert.NotEmpty(collections);

        var firstCollection = collections[0];
        Assert.Equal("test", firstCollection.GetProperty("id").GetString());
        var collectionLinks = firstCollection
            .GetProperty("links")
            .EnumerateArray()
            .ToArray();

        Assert.Contains(collectionLinks, el =>
            el.GetProperty("href").GetString() == "/collections/test" &&
            el.GetProperty("rel").GetString() == "self");
        Assert.Contains(collectionLinks, el =>
            el.GetProperty("href").GetString() == "/collections/test/items" &&
            el.GetProperty("rel").GetString() == "items" &&
            el.GetProperty("type").GetString() == "application/geo+json");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Links_NullProperties_AreOmittedFromSerialization()
    {
        var response = await this.client.GetAsync("/collections");
        var rawJson = await response.Content.ReadAsStringAsync();
        
        // Ensure "type": null or "title": null are omitted when writing null
        Assert.DoesNotContain("\"type\":null", rawJson.Replace(" ", ""));
        Assert.DoesNotContain("\"title\":null", rawJson.Replace(" ", ""));
    }
}
