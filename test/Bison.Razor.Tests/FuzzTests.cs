//using Bison.Core;

using Bison.Core.Models;
using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Repositories;
using Bison.Infrastructure.Services;

namespace Bison.Razor.Tests;


public class FuzzTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetObservations_returningExpectedDtos(int seed)
    {
        //Arrange
        var random = new Random(seed);
        string[] authors = { "Anne", "Theo", "Line", "Lærke", "Nicklas" };
        string[] messages = { "A bird", "En hejre", "A bird near the lake" };

        var observations = new List<Observation>();
        var expected = new List<ObservationDto>();

        for (int i = 0; i < 50; i++)
        {
            string author = authors[random.Next(authors.Length)];
            string message = messages[random.Next(messages.Length)];
            DateTime timestamp = new DateTime(2026, 1, 1)
                .AddMinutes(random.Next(100_000));

            var observation = new Observation
            {
                PostId = i + 1,
                Author = new Author { Name = author },
                Text = message,
                TimeStamp = timestamp
            };

            observations.Add(observation);
            expected.Add(DtoMapper.ToObservationDto(observation));
        }
        var repository = new ObservationTestRepository(observations);
        var service = new ObservationService(repository);
        //Act
        var result = service.GetObservations();

        //Assert
        Assert.Equal(expected.Count, result.Count);

        for (var i = 0; i < expected.Count; i++)
            {
            Assert.Equal(expected[i].Id, result[i].Id);
            Assert.Equal(expected[i].AuthorName, result[i].AuthorName);
            Assert.Equal(expected[i].Text, result[i].Text);
            Assert.Equal(expected[i].TimeStamp, result[i].TimeStamp);
            Assert.Equal(expected[i].DanishVernacularName, result[i].DanishVernacularName);
            Assert.Equal(expected[i].PictureUrl, result[i].PictureUrl);
        }

    
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetComments_ReturnsExpectedViewModels(int seed)
    {
        // Arrange

        var random = new Random(seed);
        const int observationId = 7;


        string[] authors = { "Anne", "Theo", "Line", "Lærke", "Nicklas" };
        string[] messages = { "En hejre?", "A bird", "Flot observation!" };

        var comments = new List<Comment>();
        var expected = new List<CommentDto>();

        for (int i = 0; i < 50; i++)
        {
            string author = authors[random.Next(authors.Length)];
            string message = messages[random.Next(messages.Length)];
            DateTime timestamp = new DateTime(2026, 1, 1)
                .AddMinutes(random.Next(100_000));

            var comment = new Comment
            {
                PostId = i + 1,
                ObservationId = observationId,
                Author = new Author { Name = author },
                Text = message,
                TimeStamp = timestamp
            };

            comments.Add(comment);

            expected.Add(DtoMapper.ToCommentDto(comment));
        }

        var repository = new ObservationTestRepository(comments);
        var service = new ObservationService(repository);


        // Act
        var result = service.GetComments(observationId);

        // Assert
        Assert.Equal(expected.Count, result.Count);

        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, result[i].Id);
            Assert.Equal(expected[i].AuthorName, result[i].AuthorName);
            Assert.Equal(expected[i].Text, result[i].Text);
            Assert.Equal(expected[i].TimeStamp, result[i].TimeStamp);
        }
    }
    private sealed class ObservationTestRepository : IPostRepository{
    private readonly List<Observation> _observations;
    private readonly List<Comment> _comments = new();
    public ObservationTestRepository(List<Observation> observations){
        _observations = observations;
    }

    public ObservationTestRepository(List<Comment> comments){
    _observations = new List<Observation>();
    _comments = comments;
    }

    public IEnumerable<ObservationDto> GetObservations(int page) =>
            _observations.Select(DtoMapper.ToObservationDto);

    public IEnumerable<CommentDto> GetComments(int observationId) =>
            _comments
                .Where(c => c.ObservationId == observationId)
                .Select(DtoMapper.ToCommentDto);

    public IEnumerable<ObservationDto> GetObservationsByAuthor(string author, int page) =>
            _observations
                .Where(o => o.Author.Name == author)
                .Select(DtoMapper.ToObservationDto);

    public IEnumerable<ProposalDto> GetProposals(int observationId) =>
            throw new NotImplementedException();
    public ObservationDto? GetObservationDetails(int id) =>
        throw new NotImplementedException();

    public Observation? GetObservationEntity(int id) =>
        _observations.FirstOrDefault(o=>o.PostId==id);
    
    public Author? GetAuthorByName(string name) =>
            throw new NotImplementedException();


    public Post GetPost(int id) =>
            throw new NotImplementedException();

    public void DeletePost(int post)
    => throw new NotImplementedException();
    public void Dispose() { }
    public void InsertPost(Post post) { }
    
    public void UpdatePost(Post post) { }
   
    public void Save() { }
   
    public Taxon? GetTaxon(int id) => throw new NotImplementedException();

    public List<Observation> GetAllObservations() => _observations;

    // Disse testklasser bruges til andre funktioner end oprettelse.
        public IReadOnlyList<Taxon> GetTaxons() =>
        throw new NotImplementedException();
    }
}

