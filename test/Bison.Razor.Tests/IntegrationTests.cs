using Bison.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bison.Razor.Tests;

// ARRANGE sker i TestDatabase.cs. den er bygget af fixturen ud fra schema.sql + dump.sql.
// Derfor behøver vi ikke at ARRANGE for hver test. Vi kan ACT og ASSERT ud fra den eksisterende test-database. 
// Ændrer eller tilføjer vi i schema og/eller dump, skal disse tests også rettes.
public class IntegrationTests : IClassFixture<TestDatabase>
{
    // Den DBFacade, testene kalder - peger på testdatabasen i stedet for bison.db.
    private readonly TestWebFactory _factory;

    public IntegrationTests(TestWebFactory factory)
    {
        // Ingen DI-container her: vi laver selv DBFacade og giver den testdatabasens sti.
        _factory=factory;
    }

    [Fact]
    public void GetObservations_ReturnsObservationsFromDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPostRepository>();

        var observations = repository.GetObservations(1);

        // ASSERT
        Assert.Contains(observations, o => o.AuthorName == "Yuki" &&
            o.Text == "A Least Bittern resting on a sandbank. Stays within cover almost all the time.");
    }

    [Fact]
    public void GetObservationsFromAuthor_ReturnsOnlyThatAuthor()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IPostRepository>();
        // ACT
    
        var observations = repo.GetObservationsByAuthor("yuki", 1);

        // ASSERT
        // Alle observationer skal være Yukis.
        Assert.NotEmpty(observations);
        Assert.All(observations, o => Assert.Equal("Yuki", o.AuthorName));
    }
}
