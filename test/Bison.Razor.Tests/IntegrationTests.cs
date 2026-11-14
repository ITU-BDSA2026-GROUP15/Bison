using Bison.Core.Models;
using Bison.Infrastructure.Data;
using Bison.Infrastructure.Repositories;

namespace Bison.Razor.Tests;

// Integrationstests af PostRepository mod en rigtig (in-memory) SQLite-database.
// ARRANGE sker i SeedData: testene forventer, at Yuki og Johan hver har én observation.
// Ændrer vi i SeedData, skal disse tests også rettes.
// IntegrationTests.cs bruger ikke længere TestDatabase eller Dbfacade. 
public class IntegrationTests : IClassFixture<InMemoryDb>, IDisposable
{
    // Det repository, testene kalder - peger på in-memory databasen.
    private readonly PostRepository _repo;

    public IntegrationTests(InMemoryDb db)
    {
        // Constructoren kører for hver test, men databasen deles af hele klassen.
        // Derfor seeder vi kun, hvis databasen er tom.
        using (var context = db.CreateContext())
        {
            if (!context.Authors.Any())
            {
                SeedData(context);
            }
        }

        // Ingen DI-container her: vi laver selv repository'et med en frisk context.
        _repo = new PostRepository(db.CreateContext());
    }

    // Lægger en observation fra Yuki og en fra Johan ind.
    // Johans observation sikrer, at filtreringen på forfatter faktisk bliver testet.
    private static void SeedData(BisonDBContext context)
    {
        var yuki = new Author
        {
            Name = "Yuki",
            Email = "yuki@example.test"
        };

        var johan = new Author
        {
            Name = "Johan",
            Email = "johan@example.test"
        };

        var taxon = new Taxon
        {
            dwc_TaxonID = "test-taxon",
            VernacularName = "Testfugl"
        };

        context.Observations.AddRange(
            new Observation
            {
                Author = yuki,
                Taxon = taxon,
                Text = "A Least Bittern resting on a sandbank. Stays within cover almost all the time.",
                TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
            },
            new Observation
            {
                Author = johan,
                Taxon = taxon,
                Text = "Great Egret on the pond at the edge of town. Nests in colonies, often in trees near water.",
                TimeStamp = new DateTime(2026, 1, 2, 12, 0, 0)
            });

        context.SaveChanges();
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
