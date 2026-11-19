using Bison.Core.Models;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;

namespace Bison.Razor.Tests;

public class ObservationCreationIntegrationTest
{
    [Fact] //fact gør at vi kan lave testmetoden uden parametre. Markerer metoden som en xUnit-test uden testdata.
    public void tryCreateObservationThatAppearsInPublicAndAuthorLists()
    {
        //arrange - InMemoryDb giver en tom, isoleret database med tabellerne oprettet.
        // Testen skriver data, så den får sin egen database.
        using var db = new InMemoryDb();
        using var writeContext = db.CreateContext();

        var taxon = new Taxon
        {
            TaxonId = 1,
            VernacularName = "Test-solsort"
        };

        writeContext.Taxons.Add(taxon);
        writeContext.SaveChanges();

        using var writeRepository = new PostRepository(writeContext);
        var writeService = new ObservationService(writeRepository);

        const string authorName = "Anne";
        const string authorEmail = "anne@example.test";
        const string observationText = "Jeg så en solsort i haven";

        //act - Opret en observation for en forfatter, der endnu ikke findes.
        var created = writeService.TryCreateObservation(authorName , authorEmail, observationText,  taxon.TaxonId);

        // Assert: Læs tilbage med en ny context på samme database,
        // så vi kontrollerer det, der faktisk blev gemt.
        Assert.True(created);

        using var readContext = db.CreateContext();
        using var readRepository = new PostRepository(readContext);
        var readService = new ObservationService(readRepository);

        var publicObservation = Assert.Single(readService.GetObservations());
        var authorObservation = Assert.Single(
            readService.GetObservationsFromAuthor(authorName));

        Assert.Equal(authorName, publicObservation.Author);
        Assert.Equal(observationText, publicObservation.Message);
        Assert.Equal(publicObservation, authorObservation);
    }
}
