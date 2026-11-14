using System.Text.RegularExpressions;
using Bison.Core.Models;
using Bison.Infrastructure.Data;

namespace Bison.Razor.Tests;

// IClassFixture gør, at alle tests i klassen deler én in-memory database og én webapp.
// Det er sikkert, fordi testene kun læser data.
public class PaginationTests : IClassFixture<InMemoryDb>
{
    private readonly HttpClient _client;

    public PaginationTests(InMemoryDb db)
    {
        // Testdata skal ind FØR appen starter. Program.cs seeder kun databasen,
        // hvis der ikke allerede findes forfattere og posts.
        using (var context = db.CreateContext())
        {
            // Constructoren kører for hver test, men databasen deles.
            // Derfor seeder vi kun, hvis databasen er tom.
            if (!context.Authors.Any())
            {
                SeedData(context);
            }
        }

        // Klienten sender HTTP-kald til testappen uden en rigtig netværksport.
        _client = db.CreateWebApp().CreateClient();
    }

    // Lægger 69 observationer ind: 65 fra Peter og 4 fra Paul.
    private static void SeedData(BisonDBContext context)
    {
        var peter = new Author
        {
            Name = "Peter",
            Email = "peter@example.test"
        };

        var paul = new Author
        {
            Name = "Paul",
            Email = "paul@example.test"
        };

        var taxon = new Taxon
        {
            dwc_TaxonID = "test-taxon",
            VernacularName = "Testfugl"
        };

        for (int id = 1; id <= 69; id++)
        {
            context.Observations.Add(new Observation
            {
                PostId = id,
                Author = id <= 65 ? peter : paul,
                Taxon = taxon,
                Text = $"PAGETEST_{id:D3}",
                TimeStamp = new DateTime(2026, 1, 1)
            });
        }

        context.SaveChanges();
    }

    // Henter en side og returnerer numrene fra testobservationernes tekster.
    private async Task<int[]> GetIds(string url)
    {
        var response = await _client.GetAsync(url);

        // Testen fejler, hvis serveren svarer med en fejlkode.
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        // Finder fx PAGETEST_065 og omdanner 065 til tallet 65.
        return Regex.Matches(html, @"PAGETEST_(\d{3})")
            .Cast<Match>()
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToArray();
    }

    // Theory kører samme test for hvert sæt InlineData.
    [Theory]
    [InlineData("/obs", 69)]
    [InlineData("/obs/Peter", 65)]
    public async Task Pages_ReturnCorrectObservationsWithoutOverlap(
        string route, int total)
    {
        // Henter fire sider fra den valgte tidslinje.
        var page1 = await GetIds($"{route}?page=1");
        var page2 = await GetIds($"{route}?page=2");
        var page3 = await GetIds($"{route}?page=3");
        var page4 = await GetIds($"{route}?page=4");

        // De første to sider skal være fulde.
        Assert.Equal(32, page1.Length);
        Assert.Equal(32, page2.Length);

        // Side 3 indeholder resten: 5 offentligt eller 1 for Peter.
        Assert.Equal(total - 64, page3.Length);

        // Der er ingen observationer tilbage på side 4.
        Assert.Empty(page4);

        // Samler resultaterne fra de tre sider.
        var allIds = page1.Concat(page2).Concat(page3).ToArray();

        // Distinct fjerner dubletter. Antallet skal stadig være totalen.
        Assert.Equal(total, allIds.Distinct().Count());

        // Datoerne er ens, så observationerne skal stå efter faldende id.
        // Sammenligningen opdager også manglende eller forkerte observationer.
        var expected = Enumerable.Range(1, total).Reverse().ToArray();
        Assert.Equal(expected, allIds);
    }

    [Theory]
    [InlineData("/obs")]
    [InlineData("/obs/Peter")]
    public async Task MissingPage_ReturnsSameAsPageOne(string route)
    {
        // Sammenligner URL'en uden sidetal med en eksplicit side 1.
        var defaultPage = await GetIds(route);
        var explicitPage = await GetIds($"{route}?page=1");

        Assert.Equal(explicitPage, defaultPage);
    }

    [Fact]
    public async Task AuthorTimeline_ReturnsOnlyThatAuthorsObservations()
    {
        var ids = await GetIds("/obs/Paul");

        // Paul har kun disse fire observationer.
        Assert.Equal(new[] { 69, 68, 67, 66 }, ids);

        // Derfor skal hans anden side være tom.
        Assert.Empty(await GetIds("/obs/Paul?page=2"));
    }

    [Fact]
    public async Task UnknownAuthor_ReturnsEmptyPage()
    {
        // En forfatter uden observationer skal give en tom side uden fejl.
        Assert.Empty(await GetIds("/obs/Unknown"));
    }
}
