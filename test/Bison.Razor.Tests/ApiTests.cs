namespace Bison.Razor.Tests;

public class ApiTests : IClassFixture<TestWebFactory>
{
    // En HttpClient, der taler direkte med den in-memory app.
    private readonly HttpClient _client;

    public ApiTests(TestWebFactory factory)
    {
        // CreateClient() starter appen.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicTimelineContainsJohansNewestObservation()
    {
        // ACT
        // Sender en GET-request til den offentlige tidslinje og læser svaret.
        var response = await _client.GetAsync("/obs");
        var html = await response.Content.ReadAsStringAsync();

        // ASSERT
        // Siden skal svare OK
        response.EnsureSuccessStatusCode();

        // HTML'en skal indeholde både forfatteren og observationens tekst.
        Assert.Contains("Johan", html);
        Assert.Contains("Great Egret on the pond at the edge of town. Nests in colonies, often in trees near water.", html);
    }

    [Fact]
    public async Task PrivateTimelineContainsYukisNewestObservation()
    {
        // ACT
        // Sender en GET-request til Yukis private tidslinje og læser svaret.
        var response = await _client.GetAsync("/obs/Yuki");
        var html = await response.Content.ReadAsStringAsync();

        // ASSERT
        // Siden skal svare OK
        response.EnsureSuccessStatusCode();

        // HTML'en skal indeholde både forfatteren og observationens tekst.
        Assert.Contains("Yuki", html);
        Assert.Contains("A Least Bittern resting on a sandbank. Stays within cover almost all the time.", html);
    }
}
