namespace Bison.Razor.Tests;

// Dump og schema stemmer ikke overens med project description. Jeg kan ikke lave testene som de står beskrevet.
public class ApiTests : IClassFixture<TestWebFactory>
{
    // En HttpClient, der taler direkte med den in-memory app.
    private readonly HttpClient _client;

    public ApiTests(TestWebFactory factory)
    {
        // CreateClient() starter appen.
        _client = factory.CreateClient();
    }

    // GET /obs skal indeholde Peters "A big gray bird in a pond at DR byen".
    [Fact(Skip = "Ikke skrevet endnu")]
    public async Task PublicTimeline_ContainsPetersObservation()
    {
    }

    // GET /obs/Petra skal indeholde Petras "A heron".
    [Fact(Skip = "Ikke skrevet endnu")]
    public async Task PrivateTimeline_Petra_ContainsHeron()
    {
    }
}
