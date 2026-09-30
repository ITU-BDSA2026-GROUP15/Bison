using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        using (var connection = new SqliteConnection(
            $"Data Source={_dbPath};Pooling=False"))
        {
            connection.Open();

            // Opretter de tabeller og brugere, som testene skal bruge.
            using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE user (
                    user_id INTEGER PRIMARY KEY,
                    username TEXT NOT NULL,
                    email TEXT NOT NULL
                );

                CREATE TABLE observation (
                    observation_id INTEGER PRIMARY KEY,
                    author_id INTEGER NOT NULL,
                    text TEXT NOT NULL,
                    pub_date INTEGER NOT NULL
                );

                INSERT INTO user VALUES
                    (1, 'Peter', 'peter@example.test'),
                    (2, 'Paul', 'paul@example.test');
                """;
            command.ExecuteNonQuery();

            // Opretter 69 observationer: 65 fra Peter og 4 fra Paul.
            for (int id = 1; id <= 69; id++)
            {
                using var insert = connection.CreateCommand();
                insert.CommandText = """
                    INSERT INTO observation
                        (observation_id, author_id, text, pub_date)
                    VALUES (@id, @author, @text, @date);
                    """;

                // Parametrene indsætter værdierne i SQL-forespørgslen.
                insert.Parameters.AddWithValue("@id", id);

                // Id 1-65 tilhører Peter. De sidste fire tilhører Paul.
                insert.Parameters.AddWithValue("@author", id <= 65 ? 1 : 2);

                // En unik tekst gør observationen genkendelig i HTML'en.
                // D3 formaterer eksempelvis 1 som 001.
                insert.Parameters.AddWithValue("@text", $"PAGETEST_{id:D3}");

                // Samme dato kontrollerer, at id bruges som ekstra sortering.
                insert.Parameters.AddWithValue("@date", 1690895308);

                insert.ExecuteNonQuery();
            }
        }

        // Opretter en testversion af webappen.
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Erstatter appens DBFacade med en, der bruger testdatabasen.
                    services.RemoveAll<DBFacade>();
                    services.AddSingleton(new DBFacade(_dbPath));
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