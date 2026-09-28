namespace Bison.Razor.Tests;

// ARRANGE sker i TestDatabase.cs. den er bygget af fixturen ud fra schema.sql + dump.sql.
// Derfor behøver vi ikke at ARRANGE for hver test. Vi kan ACT og ASSERT ud fra den eksisterende test-database. 
// Ændrer eller tilføjer vi i schema og/eller dump, skal disse tests også rettes.
public class IntegrationTests : IClassFixture<TestDatabase>
{
    // Den DBFacade, testene kalder - peger på testdatabasen i stedet for bison.db.
    private readonly DBFacade _db;

    public IntegrationTests(TestDatabase testDb)
    {
        // Ingen DI-container her: vi laver selv DBFacade og giver den testdatabasens sti.
        _db = new DBFacade(testDb.DbPath);
    }

    [Fact]
    public void GetObservations_ReturnsObservationsFromDatabase()
    {
        // ACT
        var observations = _db.GetObservations();

        // ASSERT
        Assert.NotEmpty(observations);
        Assert.Contains(observations, o => o.Author == "Eduard" && o.Message == "A heron");
    }

    [Fact]
    public void GetObservationsFromAuthor_ReturnsOnlyThatAuthor()
    {
        // ACT
        var observations = _db.GetObservationsFromAuthor("Eduard");

        // ASSERT
        Assert.NotEmpty(observations);

        // Alle observationer skal være Eduards.
        Assert.All(observations, o => Assert.Equal("Eduard", o.Author));
    }
}
