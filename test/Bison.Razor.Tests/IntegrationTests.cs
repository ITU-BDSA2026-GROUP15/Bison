using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleDB;

namespace Bison.Razor.Tests;

// WebApplicationFactory<Program> kører Bison.Razor in-memory i testprocessen, uden en rigtig
// port - i modsætning til Bison.CLI.Tests, som skal starte en rigtig proces for at teste den
// fulde kæde CLI -> HTTP -> service. Her tester vi kun servicen isoleret.
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetObservations_Returns200AndJsonList()
    {
        // ACT
        var response = await client.GetAsync("/observations");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var observations = await response.Content.ReadFromJsonAsync<List<Cheep>>();
        Assert.NotNull(observations);
    }

    [Fact]
    public async Task PostObservation_Returns200()
    {
        // ARRANGE
        var newObservation = new Cheep("Tester", 0, "Test observation " + Guid.NewGuid(), 1690891760, "ITU");

        // ACT
        var response = await client.PostAsJsonAsync("/observation", newObservation);

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
