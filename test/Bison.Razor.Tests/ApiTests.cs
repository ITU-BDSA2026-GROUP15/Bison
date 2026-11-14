using Bison.Core.Models;
using Bison.Infrastructure.Data;

namespace Bison.Razor.Tests;

public class ApiTests : IClassFixture<InMemoryDb>
{
    // En HttpClient, der taler direkte med den in-memory app.
    private readonly HttpClient _client;

    public ApiTests(InMemoryDb db)
    {
        // Testdata skal ind FØR appen starter. Program.cs seeder kun databasen,
        // hvis der ikke allerede findes forfattere og posts.
        using (var context = db.CreateContext())
        {
            // Fixturen deles af alle tests i klassen, men constructoren kører for hver test.
            // Derfor seeder vi kun, hvis databasen er tom.
            if (!context.Authors.Any())
            {
                SeedData(context);
            }
        }

        // CreateClient() starter appen.
        _client = db.CreateWebApp().CreateClient();
    }

    // Lægger de kendte testdata ind, som testene nedenfor forventer.
    private static void SeedData(BisonDBContext context)
    {
        var johan = new Author
        {
            Name = "Johan",
            Email = "johan@example.test"
        };

        var yuki = new Author
        {
            Name = "Yuki",
            Email = "yuki@example.test"
        };

        var taxon = new Taxon
        {
            dwc_TaxonID = "test-taxon",
            VernacularName = "Testfugl"
        };

        context.Observations.AddRange(
            new Observation
            {
                Author = johan,
                Taxon = taxon,
                Text = "Great Egret on the pond at the edge of town. Nests in colonies, often in trees near water.",
                TimeStamp = new DateTime(2026, 1, 2, 12, 0, 0)
            },
            new Observation
            {
                Author = yuki,
                Taxon = taxon,
                Text = "A Least Bittern resting on a sandbank. Stays within cover almost all the time.",
                TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
            });

        context.SaveChanges();
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
