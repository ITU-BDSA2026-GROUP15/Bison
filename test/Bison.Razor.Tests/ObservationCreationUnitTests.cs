using Bison.Razor.Models;
using Bison.Razor.DAL;

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

        //act

        var observation = service.TryCreateObservation( author.Name , author.Email,"Jeg så en solsort i haven",  taxon.TaxonId);


        //assert

    }    

    private sealed class TestRepository : IPostRepository
    {
        private readonly Taxon _taxon;
        private readonly Author _author;

        public TestRepository(Taxon taxon, Author author)
        {
            _taxon = taxon;
            _author = author;
        }

        public bool TryCreateObservation(string authorName, string email, string text, int taxonId)
        {
            // Simulerer oprettelse af observationen i databasen.
            // Returnerer true, hvis taxonId og forfatter findes.
            return taxonId == _taxon.TaxonId && email == _author.Email;
        }

        //creates a post and returns it, so we can check if the post was created correctly
        public Post? InsertedPost { get; private set; }

        public bool SaveCalled { get; private set; }

        public Taxon? GetTaxon(int id)
        {
            // Returnerer taxonet, hvis id matcher.
            return id == _taxon.TaxonId ? _taxon : null;
        }

        public Author? GetAuthorByName(string name)
        {
            // Returnerer forfatteren, hvis navnet matcher.
            return name == _author.Name ? _author : null;
        }


        public void InsertPost(Post post)
        {
            // Gemmer den indsatte post til senere kontrol.
            InsertedPost = post;
        }
        public void Save()
        {
            // Marker, at Save er blevet kaldt.
            SaveCalled = true;
        }

        
        public void Dispose()
        {
            // Ingen ressourcer at frigive i denne testimplementering.
        }

          // Disse metoder er kun med for at implementere resten af interfacet, NotImplemtedException derfor.
        public IEnumerable<T> GetPosts<T>(int page) where T : Post =>
            throw new NotImplementedException();

        public IEnumerable<T> GetPostsByAuthor<T>(string author, int page)
            where T : Post =>
            throw new NotImplementedException();

        public Post GetPost(int id) => throw new NotImplementedException();
        public void UpdatePost(Post post) => throw new NotImplementedException();
        public void DeletePost(int id) => throw new NotImplementedException();
        public Observation? GetObservationDetails(int id) => throw new NotImplementedException();

        public IEnumerable<Comment> GetComments(int observationId) => throw new NotImplementedException();

        public IEnumerable<Proposal> GetProposals(int observationId) => throw new NotImplementedException();

        public List<Observation> GetAllObservations() => throw new NotImplementedException();

        public IReadOnlyList<Taxon> GetTaxons() =>throw new NotImplementedException();
    }


}