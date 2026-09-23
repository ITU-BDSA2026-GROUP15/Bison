using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using SimpleDB;

namespace Bison.Razor.Tests;

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly Random _random = new();

    private readonly List<Cheep> _sentObservations = new();
    private readonly List<Cheep> _sentComments = new();



    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }


    private static readonly string[] sampleAuthors = { "Anne", "Theo", "line", "Lærke", "Nicklas" };

    private static readonly string[] sampleMessages =
        { "Do", "You", "Remember", "The", "Twenty-first", "night", "of", "September" };

    private static readonly string[] sampleLocations = { "DR-Byen", "Nicklas' crib", "genbrugspladsen", "Istedgade" };

    private Cheep GenerateRandomObservation()
    {
        String author = sampleAuthors[_random.Next(sampleAuthors.Length)];
        String message = sampleMessages[_random.Next(sampleMessages.Length)];
        String location = sampleLocations[_random.Next(sampleLocations.Length)];
        long timestamp = RandomTimeStamp();

        return new Cheep(author, 0, message, timestamp, location);
    }

    private (Cheep comment, bool referenceRealObservation) GenerateRandomComment()
    {
        string author = sampleAuthors[_random.Next(sampleAuthors.Length)];
        string message = sampleMessages[_random.Next(sampleMessages.Length)];
        long timestamp = RandomTimeStamp();

        bool useValidID = _random.NextDouble() < 0.9 && _sentObservations.Count > 0;

        int id = useValidID
            ? _sentObservations[_random.Next(_sentObservations.Count)].ID
            : _random.Next(100_000, 999_999);

        return (new Cheep(author, id, message, timestamp, ""), useValidID);

    }

    private static long RandomTimeStamp()
    {
        long minUnix = 946684800;
        long maxUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return (long)(minUnix + _random.NextDouble() * (maxUnix - minUnix));
    }

    //What the fuzztest going on here
    [Fact]
    public async Task FuzzObservationsAndComments_ServerStateMatchesOracle()
    {
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            bool postObservations = _random.NextDouble() < 0.5 || _sentObservations.Count == 0;

            if (postObservations)
            {
                var observation = GenerateRandomObservation();

                var response = await _client.PostAsJsonAsync("/observation", observation);
                response.EnsureSuccessStatusCode();


                var stored = await response.Content.ReadFromJsonAsync<Cheep>();
                Assert.NotNull(stored);
                _sentObservations.Add(stored);
            }
            else
            {
                var (comment, referenceRealObservation) = GenerateRandomComment();

                var response = await _client.PostAsJsonAsync("/comment", comment);

                //Work is needed before we can continue here - since /comments does not validate ID.
                //The CLI method comment() does, but we need to make this a possibility for /comments

                response.EnsureSuccessStatusCode();
                _sentComments.Add(comment);
            }
            //ORACLE Check - with get function


        }




        var actualObservations = await _client.GetFromJsonAsync<List<Cheep>>("/observations");
        // Alternative to:
        // var response = await _client.GetAsync("/observations");
        // var json = await response.Content.ReadAsStringAsync();
        // var actualObservations = JsonSerializer.Deserialize<List<Cheep>>(json);
        // Assert.NotNull(actualObservations);

        foreach (var expected in _sentObservations)
        {
            //Assert.contains asks if there are any items in the collection that satisfies this...
            Assert.Contains(actualObservations!, actual =>
                actual.ID == expected.ID &&
                actual.Author == expected.Author &&
                actual.Observation == expected.Observation &&
                actual.Timestamp == expected.Timestamp &&
                actual.Location == expected.Location);
            // this means that the boolean only marks true if ALL actuals are equal to expected

        }

//Oracle test = GET /comments
        foreach (var observation in _sentObservations)
        {
            var expectedComments = _sentComments.Where(c => c.ID == observation.ID).ToList();

            var actualComments = await _client.GetFromJsonAsync<List<Cheep>>($"/comments?id={observation.ID}");
            Assert.NotNull(actualComments);

            Assert.Equal(expectedComments.Count, actualComments.Count);

            foreach (var expected in expectedComments)
            {
                Assert.Contains(actualComments!, actual =>

                    actual.Author == expected.Author &&
                    actual.Observation == expected.Observation &&
                    actual.Timestamp == expected.Timestamp);

            }
        }
    }
}

