using Bison.Core.Models;
using Bison.Infrastructure.Repositories;

namespace Bison.Razor.Tests;

// Integrationstests af PostRepository mod en rigtig (in-memory) SQLite-database.
// ARRANGE sker i InMemoryDb.SeedJohanAndYuki(): testene forventer, at Yuki og Johan hver har én observation.
// Johans observation sikrer, at filtreringen på forfatter faktisk bliver testet.
// Ændrer vi i SeedJohanAndYuki, skal disse tests også rettes.
// IntegrationTests.cs bruger ikke længere TestDatabase eller Dbfacade.
public class IntegrationTests : IClassFixture<InMemoryDb>, IDisposable
{
    // Det repository, testene kalder - peger på in-memory databasen.
    private readonly PostRepository _repo;

    public IntegrationTests(InMemoryDb db)
    {
        // Seeder kun første gang, da databasen deles af hele klassen.
        db.SeedJohanAndYuki();

        // Ingen DI-container her: vi laver selv repository'et med en frisk context.
        _repo = new PostRepository(db.CreateContext());
    }

    [Fact]
    public void GetObservations_ReturnsObservationsFromDatabase()
    {
        // ACT
        var observations = _repo.GetPosts<Observation>(1);

        // ASSERT
        Assert.Contains(observations, o => o.Author.Name == "Yuki" &&
            o.Text == "A Least Bittern resting on a sandbank. Stays within cover almost all the time.");
    }

    [Fact]
    public void GetObservationsFromAuthor_ReturnsOnlyThatAuthor()
    {
        // ACT
        var observations = _repo.GetPostsByAuthor<Observation>("Yuki", 1);

        // ASSERT
        // Alle observationer skal være Yukis.
        Assert.NotEmpty(observations);
        Assert.All(observations, o => Assert.Equal("Yuki", o.Author.Name));
    }

    // Lukker repository'ets context. Selve forbindelsen lukkes af InMemoryDb.
    public void Dispose()
    {
        _repo.Dispose();
    }
}
