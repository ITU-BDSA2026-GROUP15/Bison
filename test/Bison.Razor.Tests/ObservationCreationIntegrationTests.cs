using Bison.Razor.DAL;
using Bison.Razor.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Tests;



public class ObservationCreationIntegrationTest
{
    
   [Fact] //fact gør at vi kan lave testmetoden uden parametre.
    public void tryCreateObservation()
    {
        //arrange 

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();


        var options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(connection)
            .Options;

        
        using var writeContext = new BisonDBContext(options);
        writeContext.Database.EnsureCreated();


        var taxon = new Taxon
        {
            TaxonId = 1,
            VernacularName = "Test-solsort"
        };

        writeContext.Taxons.Add(taxon);
        writeContext.SaveChanges();

        using var writeRepository = new PostRepository(writeContext);
        var writeService = new ObservationService(writeRepository);



        var author = new Author
        {
            Name = "Anne",
            Email = "anne@example.test"
            
        };



        //act
        const string observationText = "Jeg så en solsort i haven";
        var created = writeService.TryCreateObservation(author.Name , author.Email, observationText,  taxon.TaxonId);


        // Assert: Læs tilbage med en ny context på samme database,
        // så vi kontrollerer det, der faktisk blev gemt.
        Assert.True(created);

        using var readContext = new BisonDBContext(options);
        using var readRepository = new PostRepository(readContext);
        var readService = new ObservationService(readRepository);

        var publicObservation = Assert.Single(readService.GetObservations());
        var authorObservation = Assert.Single(
            readService.GetObservationsFromAuthor(author.Name));

        Assert.Equal(author.Name, publicObservation.Author);
        Assert.Equal(observationText, publicObservation.Message);
        Assert.Equal(publicObservation, authorObservation);
    }

}
