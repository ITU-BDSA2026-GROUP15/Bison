using Bison.Core.Models;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;

namespace Bison.Razor.Tests;

// Hver test laver sin egen InMemoryDb, fordi testene har forskellige data
// og genbruger de samme TaxonId'er. En delt database ville blande dem sammen.
public class FilterServiceTests
{
    // Tester, at et ukendt taxon giver en tom liste.
    [Fact]
    public void getObservationsFromTaxon_unknownTaxon_returnsEmptyList()
    {
        //arrange
        // InMemoryDb giver en tom, isoleret database med tabellerne oprettet.
        // using var sørger for, at databasen lukkes, når testen slutter.
        using var db = new InMemoryDb();
        using var context = db.CreateContext();

        // Repositoryet får adgang til testdatabasen gennem context.
        using var repository = new PostRepository(context);

        // FilterService får repositoryet gennem sin konstruktør.
        var service = new FilterService(repository);

        //act - Søg efter taxon 999, som ikke findes i den tomme database.
        var result = service.GetObservationsFromTaxon(999);

        //assert - Kontrollér, at resultatet er en tom liste.
        Assert.Empty(result);
    }

    [Fact] //- Tester, at et kendt taxon giver den forventede observation.
    public void GetObservationsFromTaxon_ExactMatch_ReturnsObservation()
    {
        // InMemoryDb giver en tom, isoleret database med tabellerne oprettet.
        using var db = new InMemoryDb();
        using var context = db.CreateContext();

        // Opretter det taxon, vi senere filtrerer på.
        //vernacularName er det danske navn, som vises i viewet, fra taxon.cs
        var taxon = new Taxon
        {
            TaxonId = 1,
            VernacularName = "Ænder"
        };

        // Observationen skal have en forfatter, som servicen kan læse.
        var author = new Author
        {
            Name = "Anne",
            Email = "anne@example.test"
        };

        // Taxon = taxon knytter observationen direkte til taxonet ovenfor.
        // Author = author knytter observationen til forfatteren.
        var observation = new Observation
        {
            Taxon = taxon,
            Author = author,
            Text = "En and ved søen",
            TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
        };

        // Add markerer observationen og de tilknyttede nye objekter til indsættelse.
        context.Observations.Add(observation);

        // Gemmer observationen, taxonet og forfatteren i testdatabasen.
        context.SaveChanges();

        // Fjerner objekterne fra contextens hukommelse.
        // Repositoryet skal derfor hente data fra databasen igen.
        context.ChangeTracker.Clear();

        // Opretter repositoryet og giver det til filterservicen.
        using var repository = new PostRepository(context);
        var service = new FilterService(repository);

        // act - Søg efter taxon 1, som vi netop har oprettet i databasen.
        var result = service.GetObservationsFromTaxon(1);

        // assert - Kontrollér, at resultatet indeholder præcis én observation, og at den er den forventede.

        // Single kontrollerer, at resultatet indeholder præcis ét element.
        // Det element returneres og gemmes i actual.
        var actual = Assert.Single(result);

        // Kontrollerer, at resultatet indeholder den forventede observation.
        // Message er viewmodellens navn for observationens Text.
        Assert.Equal("En and ved søen", actual.Message);
        Assert.Equal("Anne", actual.Author);
    }

    // Tester, at observationer fra en undergruppe også kommer med i resultatet.
    [Fact]
    public void GetObservationsFromTaxon_Subtaxon_ReturnsObservation()
    {
        //arrange
        // InMemoryDb giver en tom, isoleret database med tabellerne oprettet.
        using var db = new InMemoryDb();
        using var context = db.CreateContext();

        // Opretter det overordnede taxon, som vi vil filtrere på.
        var parentTaxon = new Taxon
        {
            TaxonId = 1,
            VernacularName = "Andefugle"
        };

        // Opretter et subTaxon.
        // Parent knytter Ænder til det overordnede taxon Andefugle.
        var childTaxon = new Taxon
        {
            TaxonId = 2,
            VernacularName = "Ænder",
            Parent = parentTaxon
        };
        // Opretter et taxon, som ikke er relateret til Andefugle.
        var unrelatedTaxon = new Taxon
        {
            TaxonId = 10,
            VernacularName = "Solsort"
        };

        // Opretter observationens forfatter.
        var author = new Author
        {
            Name = "Anne",
            Email = "anne@example.test"
        };

        // Observationen knyttes til subTaxonet Ænder.
        var observation = new Observation
        {
            Taxon = childTaxon,
            Author = author,
            Text = "En and ved søen",
            TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
        };

        var unrelatedObservation = new Observation
        {
            Taxon = unrelatedTaxon,
            Author = author,
            Text = "En solsort i haven",
            TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0)
        };

        // Tilføjer observationen og de tilknyttede objekter.
        // EF Core følger også forbindelsen fra childTaxon til parentTaxon.
        context.Observations.AddRange(observation, unrelatedObservation);

        // Gemmer forfatteren, begge taxoner og observationen i databasen.
        context.SaveChanges();

        // Fjerner de gemte objekter fra contextens hukommelse.
        // Repositoryet skal selv indlæse taxonerne og deres relation.
        context.ChangeTracker.Clear();

        // Opretter repositoryet og filterservicen med testdatabasen.
        using var repository = new PostRepository(context);
        var service = new FilterService(repository);

        //act
        // Filtrerer på Andefugle, selvom observationen er knyttet til Ænder.
        var result = service.GetObservationsFromTaxon(1);

        //assert
        // Kontrollerer, at præcis én observation bliver returneret.
        var actual = Assert.Single(result);

        // Kontrollerer, at observationen fra undertaxonet er med.
        Assert.Equal("En and ved søen", actual.Message);
    }
}
