//using Bison.Core;
using Bison.Razor.Models;
using Bison.Razor.DAL;

public class ProposalTests
{
    private const string ValidTaxonId = "MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea";
    

    [Fact]
    public void GetProposal_returningExpectedViewmodel()
    {
        //Arrgange 
        var timestamp = new DateTime(2026, 1, 1, 12, 0, 0);

        var proposal = new Proposal
        {
            ObservationId = 7,
            Author = new Author { Name = "Anne" },
            Text = "A bird",
            TimeStamp = timestamp,
        };
        var repository = new ProposalTestRepository(proposal);
        var service = new ObservationService(repository);

        //Act
        var result = service.GetProposals(7);

        //Assert
      
        var actual = Assert.Single(result);

        Assert.Equal("Anne", actual.Author);
        Assert.Equal("A bird", actual.Message);
        Assert.Equal(timestamp.ToString("g"), actual.Timestamp);

    }

    [Fact]
    public void GetProposal_returningEmptyListForNonExistingObservation()
    {
        //Arrange
        var timestamp = new DateTime(2026, 1, 1, 12, 0, 0);

        var proposal = new Proposal
        {
            ObservationId = 7,
            Author = new Author { Name = "Anne" },
            Text = "A bird",
            TimeStamp = timestamp,
        };
        var repository = new ProposalTestRepository(proposal);
        var service = new ObservationService(repository);

        //Act
        var result = service.GetProposals(9999);

        //Assert
        Assert.Empty(result);
    }



    private class ProposalTestRepository : IPostRepository
    {
        private readonly Proposal _proposal;

        public ProposalTestRepository(Proposal proposal)
        {
            _proposal = proposal;
        }

        public IEnumerable<Proposal> GetProposals(int observationId)
        {
            if (_proposal.ObservationId == observationId)
            {
                return new List<Proposal> { _proposal };
            }
            return new List<Proposal>();
        }

        // Implement other methods from IPostRepository as needed, throwing NotImplementedException for unneeded methods.
        public void Dispose() { }
        public void InsertPost(Post post) => throw new NotImplementedException();
        public IEnumerable<T> GetPosts<T>(int page) where T : Post => throw new NotImplementedException();
        public IEnumerable<T> GetPostsByAuthor<T>(string author, int page) where T : Post => throw new NotImplementedException();
        public Post GetPost(int id) => throw new NotImplementedException();
        public void UpdatePost(Post post) => throw new NotImplementedException();
        public void DeletePost(int post) => throw new NotImplementedException();
        public void Save() => throw new NotImplementedException();
        public Observation? GetObservationDetails(int id) => throw new NotImplementedException();
        public IEnumerable<Comment> GetComments(int ObservationId) => throw new NotImplementedException();
    }

}

/*
public class ProposalTests
{
    private const string ValidTaxonId = "MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea";

    [Fact]
    public void ExistingObservationIsAccepted()
    {
        var observations = new[]
        {
            new Cheep("Anne", 7, "A bird", 0, "ITU")
        };

        Assert.True(ProposalValidator.ObservationExists(observations, 7));
    }

    [Fact]
    public void MissingObservationIsRejected()
    {
        var observations = new[]
        {
            new Cheep("Anne", 7, "A bird", 0, "ITU")
        };

        Assert.False(ProposalValidator.ObservationExists(observations, 9999));
    }

    [Fact]
    public void ExistingTaxonIsAccepted()
    {
        var taxonomy = new Taxonomy();

        Assert.True(ProposalValidator.TaxonExists(taxonomy, ValidTaxonId));
    }

    [Fact]
    public void MissingTaxonIsRejected()
    {
        var taxonomy = new Taxonomy();

        Assert.False(ProposalValidator.TaxonExists(taxonomy, "not-a-taxon-id"));
    }
}
*/