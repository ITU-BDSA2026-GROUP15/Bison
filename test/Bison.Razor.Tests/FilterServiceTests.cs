namespace Bison.Razor.Tests;
using Bison.Razor.DAL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;
public class FilterServiceTests
{
    // Tester, at et ukendt taxon giver en tom liste.
    [Fact]
   
    public void getObservationsFromTaxon_unknownTaxon_returnsEmptyList()
    {
        //arrange 
        //oprette en tom midlertidig database i hukommelsen

        // SqliteConnection kommer fra Microsoft.Data.Sqlite.
        // "Data Source=:memory:" betyder, at databasen ligger i hukommelsen.
        // var betyder, at C# selv udleder variablens type: SqliteConnection.
        // using var sørger for, at forbindelsen lukkes, når testen slutter.
        using var connection = new SqliteConnection("DataSource=:memory:");
        

        // Åbner forbindelsen. SQLite opretter databasen i hukommelsen.
        // Databasen eksisterer, så længe forbindelsen er åben.
        connection.Open();

        // DbContextOptionsBuilder kommer fra Microsoft.EntityFrameworkCore.
        // Den bruges til at konfigurere vores egen BisonDBContext.
        // UseSqlite vælger SQLite og giver den forbindelsen ovenfor.
        // Options giver os den færdige konfiguration.
        // Variablen options får typen DbContextOptions<BisonDBContext>.
        var options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(connection)
            .Options;

        // BisonDBContext er fra Bison.Razor.DAL.
        // Context giver adgang til fx context.Taxons og context.Observations.
        // using var sørger for, at context bliver frigivet efter testen.
        using var context = new BisonDBContext(options);

        // Opretter tabellerne. De indeholder stadig ingen data.
        // context.Database.EnsureCreated() kommer fra Microsoft.EntityFrameworkCore.
        // Hvis tabellerne allerede findes, gør EnsureCreated ingenting.
        context.Database.EnsureCreated();

        // PostRepository er klassen fra Bison.Razor.DAL.
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
    // Opretter og åbner en isoleret database i hukommelsen.
    using var connection =
        new SqliteConnection("Data Source=:memory:");
    connection.Open();

    // Konfigurerer BisonDBContext til at bruge SQLite-forbindelsen.
    var options = new DbContextOptionsBuilder<BisonDBContext>()
        .UseSqlite(connection)
        .Options;

    // Opretter context og databasens tomme tabeller.
    using var context = new BisonDBContext(options);
    context.Database.EnsureCreated();

    // Taxon er klassen fra Bison.Razor.Models.
    // Opretter det taxon, vi senere filtrerer på.
    //vernacularName er det danske navn, som vises i viewet, fra taxon.cs
    var taxon = new Taxon
    {
        TaxonId = 1,
        VernacularName = "Ænder"
    };

    // Author er klassen fra Bison.Razor.Models.
    // Observationen skal have en forfatter, som servicen kan læse.
    var author = new Author
    {
        Name = "Anne",
        Email = "anne@example.test"
    };

    // Observation er klassen fra Bison.Razor.Models.
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

    // Filtrerer på det taxon, som observationen er knyttet til.
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
        // Opretter og åbner en isoleret SQLite-database i hukommelsen.
        using var connection =
            new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Opsætter BisonDBContext til at bruge testdatabasen.
        var options = new DbContextOptionsBuilder<BisonDBContext>()
            .UseSqlite(connection)
            .Options;

        // opretter context og de tomme tabeller.
        using var context = new BisonDBContext(options);
        context.Database.EnsureCreated();

      
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

        // Tilføjer observationen og de tilknyttede objekter.
        // EF Core følger også forbindelsen fra childTaxon til parentTaxon.
        context.Observations.Add(observation);

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