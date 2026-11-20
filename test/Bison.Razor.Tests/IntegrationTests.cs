using Bison.Infrastructure.Repositories;

namespace Bison.Razor.Tests;

// Integrationstests af PostRepository mod en rigtig (in-memory) SQLite-database.
// ARRANGE sker i InMemoryDb.SeedJohanAndYuki(): testene forventer, at Yuki og Johan hver har én observation.
// Johans observation sikrer, at filtreringen på forfatter faktisk bliver testet.
// Ændrer vi i SeedJohanAndYuki, skal disse tests også rettes.
// IntegrationTests.cs bruger ikke længere TestDatabase eller Dbfacade.
public class IntegrationTests : IClassFixture<InMemoryDb>, IDisposable
{
    // Den DBFacade, testene kalder - peger på testdatabasen i stedet for bison.db.
    private readonly PostRepository _repo;

    public IntegrationTests(InMemoryDb testDb)
    {
        // Ingen DI-container her: vi laver selv DBFacade og giver den testdatabasens sti.
       testDb.SeedJohanAndYuki();

       _repo=new PostRepository(testDb.CreateContext());
    }

    [Fact]
    public void GetObservations_ReturnsObservationsFromDatabase()
    {
        // ACT
        var observations = _repo.GetObservations(1);

        // ASSERT
        Assert.Contains(observations, o => 
        o.AuthorName == "Yuki" &&
        o.Text == "A Least Bittern resting on a sandbank. Stays within cover almost all the time.");
    }

    [Fact]
    public void GetObservationsFromAuthor_ReturnsOnlyThatAuthor()
    {
        var observations = _repo.GetObservationsByAuthor("Yuki", 1);

        // ASSERT
        // Alle observationer skal være Yukis.
        Assert.NotEmpty(observations);
        Assert.All(observations, o => Assert.Equal("Yuki", o.AuthorName));
    }


public void Dispose()
    {
        _repo.Dispose();
    }

}