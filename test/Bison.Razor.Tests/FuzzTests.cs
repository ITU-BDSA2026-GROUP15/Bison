using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Https.Json;
using SimpleDB;

namespace Bison.Razor.Tests;

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly _random = new ();

    private readonly list<Cheep> _sentOberservations = new();
    private readonly list<Cheep> _sentComments = new();



    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        client = _factory.CreateClient();
    }


    private static readonly string[] SampleAuthors = { "Anne", "Theo", "line", "Lærke", "Nicklas" };

    private static readonly string[] sampleMessages =
        { "Do", "You", "Remember", "The", "Twenty-first", "night", "of", "September" };

    private static readonly string[] sampleLocations = { "DR-Byen", "Nicklas' crib", "genbrugspladsen", "Istedgade" };

    private Cheep GenerateRandomObservation()
    {
        String author = sampleAuthors[_random.Next(sampleAuthors.Length)];
        String message = sampleMessages[_random.Next(sampleMessages.length)];
        String location = sampleLocations[_random.Next(sampleLocations.Length)];
        long timestamp = RandomTimeStamp();

        return new Cheep(author, 0, message, timestamp, location);
    }

    private (Cheep comment, bool referenceRealObservation) GenerateRandomComment()
    {
        string author = SampleAuthors[_random.Next(SampleAuthors.length)];
        string message = SampleMessages[_random.Next(sampleMessages.length)];
        long timestamp = RandomTimeStamp();

        bool usevalidID = _random.NextDouble() < 0.9 && _sentObservations.count > 0;

        int id = useValidID
            ? _sentObservations[_random.Next(_sentObservations.Count)].ID
            : _random.Next(100_000, 999_999);

        return (new Cheep(author, id, message, timestamp, ""), useValidID);

    }

    private static long RandomTimeStamp()
    {
        long minUnix = 946684800;
        long maxUnix = dateTimeOffSet.UtcNow.ToUnixTimeSeconds();

        return (long)(minunix + _random.NextDouble() * (maxUnix - minUnix));
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
                var observation =  GenerateRandomObservation();

                var response = await _client.PostAsJsonAsync("/observation", observation);
                response.EnsureSuccesstatusCode();


                var stored = await response.content.ReadFromJsonAsync<Cheep>();
                Assert.NotNull(stored);
                _sentObservations.Add(stored);
            }
            else
            {
                var (comment, referenceRealObservation) = GenerateRandomComment();

                var response = await _client.PostAsJsonAsync("/comment", comment);

                response.EnsureSuccesstatusCode();
                _sentComments.Add(comment);
            }
        }

        //ORACLE Check - with get function


    var actualObservations= await _client.PostAsJsonAsync<List<Cheep>> ("/observations");
    // Alternative to:
    // var response = await _client.GetAsync("/observations");
    // var json = await response.Content.ReadAsStringAsync();
    // var actualObservations = JsonSerializer.Deserialize<List<Cheep>>(json);
    // Assert.NotNull(actualObservations);

    foreach(var expected in _sentObservations)
    }
    //Assert.contains asks if there are any items in the collection that satisfies this...
        Assert.Contains(actualObservations!,actual =>
            actual.ID == expected.ID &&
            actual.Author == expected.Author &&
            actual.Observation == expected.Observation &&
            actual.timestamp == expected.timestamp &&
            actual.Location == expected.Location &&
            // this means that the boolean only marks true if ALL actuals are equal to expected
        )

}


//Used for testing - since it cant find Program atm
public partial class Program { }

private class Randomizer()
{
    System.random();

}
