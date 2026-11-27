using Bison.Core.Models;
using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Repositories;

namespace Bison.Razor.Tests;
internal sealed class TestRepository : IPostRepository
    {
        private readonly Taxon _taxon;
        private readonly Author _author;

        private readonly Observation? _o;

        public TestRepository(Taxon taxon, Author author, Observation? o=null)
        {
            _taxon = taxon;
            _author = author;
            _o = o;
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
        public IEnumerable<ObservationDto> GetObservations(int page) =>
        throw new NotImplementedException();

        public IEnumerable<ObservationDto> GetObservationsByAuthor(string author, int page) =>
        throw new NotImplementedException();

        public ObservationDto? GetObservationDetails(int id) 
            => throw new NotImplementedException();
        
        public IEnumerable <CommentDto> GetComments( int ObservationId) =>
             throw new NotImplementedException();

        public IEnumerable <ProposalDto> GetProposals(int ObservationId) =>
            throw new NotImplementedException();
    
        public Observation? GetObservationEntity(int id)
    {
        return _o is not null && id == _o.PostId ? _o: null;
    }

        public Post GetPost(int id) => throw new NotImplementedException();
        public void UpdatePost(Post post) => throw new NotImplementedException();
        public void DeletePost(int id) => throw new NotImplementedException();
        

        public List<Observation> GetAllObservations() => throw new NotImplementedException();

        public IReadOnlyList<Taxon> GetTaxons() =>throw new NotImplementedException();
    }
