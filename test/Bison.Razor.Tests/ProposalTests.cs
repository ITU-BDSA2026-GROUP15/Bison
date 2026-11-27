//using Bison.Core;

using Bison.Core.Models;
using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;

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

        Assert.Equal("Anne", actual.AuthorName);
        Assert.Equal("A bird", actual.Text);
        Assert.Equal("2026-01-01 12:00", actual.TimeStamp);

    }

    [Fact]
    public void GetProposal_returningEmptyListForNonExistingObservation()
    {
       var proposal = new Proposal
         {
            ObservationId = 7,
            Author = new Author { Name = "Anne" },
            Text = "A bird",
            TimeStamp = new DateTime(2026, 1, 1, 12, 0, 0),
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

        public IEnumerable<ProposalDto> GetProposals(int observationId)
        {
            if (_proposal.ObservationId == observationId)
            {
                return new[] { DtoMapper.ToProposalDto(_proposal) };
            }

            return Enumerable.Empty<ProposalDto>();
        }


        // Implement other methods from IPostRepository as needed, throwing NotImplementedException for unneeded methods.
        public IEnumerable<ObservationDto> GetObservations(int page) =>
            throw new NotImplementedException();

        public IEnumerable<ObservationDto> GetObservationsByAuthor(string author, int page) =>
            throw new NotImplementedException();

        public ObservationDto? GetObservationDetails(int id) =>
            throw new NotImplementedException();

        public IEnumerable<CommentDto> GetComments(int observationId) =>
            throw new NotImplementedException();

        public Observation? GetObservationEntity(int id) =>
            throw new NotImplementedException();

        public Taxon? GetTaxon(int id) =>
            throw new NotImplementedException();

        public Author? GetAuthorByName(string name) =>
            throw new NotImplementedException();

        public IReadOnlyList<Taxon> GetTaxons() =>
            throw new NotImplementedException();

        public Task<Dictionary<int, (int LikeCount, bool IsLikedByCurrentUser)>>
            GetLikeStatusesAsync(int[] observationIds, string? currentUserId)
        {
            return Task.FromResult(
                new Dictionary<int, (int LikeCount, bool IsLikedByCurrentUser)>());
        }

        public Task<bool> SetLikeAsync(
            int observationId, string userId, bool isLiked)
        {
            return Task.FromResult(false);
        }
        public void InsertPost(Post post) =>
            throw new NotImplementedException();

        public Post GetPost(int id) =>
            throw new NotImplementedException();

        public void UpdatePost(Post post) =>
            throw new NotImplementedException();

        public List<Observation> GetAllObservations() =>
            throw new NotImplementedException();

        public void DeletePost(int post) =>
            throw new NotImplementedException();

        public void Save() =>
            throw new NotImplementedException();

        public void Dispose() { }

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
