using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Bison.Razor.DAL;
using Bison.Razor.Models;
using Microsoft.EntityFrameworkCore;
namespace Bison.Razor.Tests;

// IDisposable gør, at xUnit kalder Dispose efter hver test.
public class PaginationTests : IDisposable
{
    private readonly string _dbPath;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PaginationTests()
    {
        // Et unikt filnavn giver hver test sin egen database.
        _dbPath = Path.Combine(
            Path.GetTempPath(),
            $"bison-pagination-{Guid.NewGuid()}.db");

        // Åbner testdatabasen. SQLite opretter filen, hvis den ikke findes.
        // Pooling=False lukker denne forbindelse helt efter brug.
       var options = new DbContextOptionsBuilder<BisonDBContext>()
    .UseSqlite($"Data Source={_dbPath};Pooling=False")
    .Options;

using (var context = new BisonDBContext(options))
{
    context.Database.EnsureCreated();

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
        // Opretter en testversion af webappen.
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Lader webappen bruge EF Core-testdatabasen.
                    services.RemoveAll<BisonDBContext>();
                    services.RemoveAll<DbContextOptions<BisonDBContext>>();
                    services.AddDbContext<BisonDBContext>(options =>
                        options.UseSqlite($"Data Source={_dbPath};Pooling=False"));
                    
                });
            });

        // Klienten sender HTTP-kald til testappen uden en rigtig netværksport.
        _client = _factory.CreateClient();
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

    public void Dispose()
    {
        // Lukker klienten og testappen efter testen.
        _client.Dispose();
        _factory.Dispose();

        // Frigiver gemte SQLite-forbindelser til denne testdatabase.
        using var poolConnection = new SqliteConnection(
            $"Data Source={_dbPath}");
        SqliteConnection.ClearPool(poolConnection);

        // Sletter kun den midlertidige database, som testen oprettede.
        File.Delete(_dbPath);
    }
}