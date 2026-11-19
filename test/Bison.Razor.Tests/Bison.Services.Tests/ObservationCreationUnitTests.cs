using Bison.Core.Models;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;

namespace Bison.Razor.Tests;

public class ObservationCreationUnitTest
{


[Fact]
 public void tryCreateObservationFromVaildTaxon()
    {
        //arrange - reporsitorty kender allerede til taxonet og forfatteren
        //så vi kan oprette en observation med et eksisterende taxon og forfatter

        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        using var repository = new TestRepository(taxon, author);
        var service = new ObservationService(repository);

        //act - forsøger at oprette en observation ud fra taxon og forfatteren, som repository kender til.

        var created = service.TryCreateObservation( author.Name , author.Email,"Jeg så en solsort i haven",  taxon.TaxonId);


        //assert - forventer, at servicen returnerer true og sender en
        // observation med det valgte taxon, den rigtige forfatter og den
        // indtastede tekst til repositoryet.forventer også, at Save() kaldes.

        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(created,"Observation was created successfully");
        var observation = Assert.IsType<Observation>(repository.InsertedPost);
        Assert.Same(taxon, observation.Taxon);
        Assert.Same(author, observation.Author);
        Assert.Equal("Jeg så en solsort i haven", observation.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void tryCreateObservationFromUnknownTaxon()
    {
        //arrange - reporsitorty kender allerede til taxonet og forfatteren
        //så vi kan oprette en observation med et eksisterende taxon og forfatter

        var taxon = new Taxon{TaxonId = 1,};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};
        using var repository = new TestRepository(taxon, author);
        var service = new ObservationService(repository);
        //act - 999 er et ukendt taxon id
        var created = service.TryCreateObservation( author.Name , author.Email,"Jeg så en fugl",  999);




        //assert - AssertFalse tester om servicen returnerer false,
        // og at repository ikke modtager nogen observation og Save() ikke kaldes.

        //Assert.Null tester om repository.InsertedPost er null,
        // hvilket betyder, at ingen observation blev oprettet.
        Assert.False(created,"Observation was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);

    }
}