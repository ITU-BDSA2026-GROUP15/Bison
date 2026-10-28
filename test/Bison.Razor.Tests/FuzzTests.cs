//using Bison.Core;
using Bison.Razor.Models;
using Bison.Razor.DAL;
namespace Bison.Razor.Tests;


public class FuzzTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetObservations_returningExpectedViewmodel(int seed)
    {
        //Arrange
        var random = new Random(seed);
        string[] authors = { "Anne", "Theo", "Line", "Lærke", "Nicklas" };
        string[] messages = { "A bird", "En hejre", "A bird near the lake" };

        var observations = new List<Observation>();
        var expected = new List<ObservationViewModel>();

        for (int i = 0; i < 50; i++)
        {
            string author = authors[random.Next(authors.Length)];
            string message = messages[random.Next(messages.Length)];
            DateTime timestamp = new DateTime(2026, 1, 1)
                .AddMinutes(random.Next(100_000));

            observations.Add(new Observation
            {
                PostId = i + 1,
                Author = new Author { Name = author },
                Text = message,
                TimeStamp = timestamp
            });

            expected.Add(new ObservationViewModel(
                author,
                message,
                timestamp.ToString("g")));
        }
        var repository = new ObservationTestRepository(observations);
        var service = new ObservationService(repository);
        //Act
        var result = service.GetObservations();

        //Assert
        Assert.Equal(expected, result);
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
        var expected = new List<ObservationViewModel>();

        for (int i = 0; i < 50; i++)
        {
            string author = authors[random.Next(authors.Length)];
            string message = messages[random.Next(messages.Length)];
            DateTime timestamp = new DateTime(2026, 1, 1)
                .AddMinutes(random.Next(100_000));

            comments.Add(new Comment
            {
                PostId = i + 1,
                ObservationId = observationId,
                Author = new Author { Name = author },
                Text = message,
                TimeStamp = timestamp
            });

            expected.Add(new ObservationViewModel(
                author,
                message,
                timestamp.ToString("g")));
        }

        var repository = new ObservationTestRepository(comments);
        var service = new ObservationService(repository);
        

        // Act
        var result = service.GetComments(observationId);

        // Assert
        Assert.Equal(expected, result);
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
    public IEnumerable<Comment> GetComments(int observationId) {
        return _comments.Where(c => c.ObservationId == observationId);
    }

    public IEnumerable<T> GetPosts<T>(int page) where T : Post
    {
        return _observations.OfType<T>();
    }
    public void DeletePost(int post)
    => throw new NotImplementedException();
    public void Dispose() { }
    public void InsertPost(Post post) { }
    public Post GetPost(int id) => throw new NotImplementedException();
    public IEnumerable<T> GetPostsByAuthor<T>(string author, int page) where T : Post => throw new NotImplementedException();
    public void UpdatePost(Post post) { }
    public void DeletePost(Post post) { }
    public void Save() { }
    public Observation? GetObservationDetails(int id) => throw new NotImplementedException();
    public IEnumerable<Proposal> GetProposals(int ObservationId) => throw new NotImplementedException();
    
    public Taxon? GetTaxon(int id) => throw new NotImplementedException();

    public List<Observation> GetAllObservations() => throw new NotImplementedException();
    }
}

