using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using SimpleDB;
using Bison.Taxonomy;

namespace Bison.Razor.Tests;

//Theory and Inlign data needs to be used here

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
    // IClassFixture<WebApplicationFactory<Program>> Tells the Xunit to boot Razor once, in-memory and share the same instance
    // instead of starting a fresh app per test,
{
    private readonly HttpClient _client;
    private Random _random = new();

    private readonly List<Cheep> _sentObservations = new();
    private readonly List<Cheep> _sentComments = new();

    private readonly List<Cheep> _sentProposals = new();


    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }


    private static readonly string[] sampleAuthors = { "Anne", "Theo", "line", "Lærke", "Nicklas" };

    private static readonly string[] sampleMessages =
        { "Do", "You", "Remember", "The", "Twenty-first", "night", "of", "September" };

    private static readonly string[] sampleLocations = { "DR-Byen", "Nicklas' crib", "genbrugspladsen", "Istedgade" };


    // Builds one random observation to POST to /observation.
    // ID is set to 0 here because /observation ignores whatever ID the client sends
    // and assigns its own (see Bison.Razor/Program.cs), so this is used as a placeholder

    private Cheep GenerateRandomObservation()
    {
        String author = sampleAuthors[_random.Next(sampleAuthors.Length)];
        String message = sampleMessages[_random.Next(sampleMessages.Length)];
        String location = sampleLocations[_random.Next(sampleLocations.Length)];
        long timestamp = RandomTimeStamp();

        return new Cheep(author, 0, message, timestamp, location);
    }
    // Builds one random comment to POST to /comment.
    // Returns a tuple: the comment itself, plus a flag telling the caller whether
    // we deliberately gave it a REAL observation ID or a fake/invalid one - useful
    // once /comment starts validating IDs and we need to assert different outcomes.

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

    private (Prop proposal, bool referenceRealObservation, bool referenceRealProposal) GenerateRandomProposal()
    {
        string author = sampleAuthors[_random.Next(sampleAuthors.Length)];
        bool useValidID = _random.NextDouble() < 0.9 && _sentObservations.Count > 0;

        int id = useValidID
            ? _sentObservations[_random.Next(_sentObservations.Count)].ID
            : _random.Next(100_000, 999_999);
        
        List<Taxon> taxons = Bison.Taxonomy.Taxonomy.ReadTaxonsFromResource();
        bool useRealTaxonId = taxons.Count > 0;

        string TaxonId = useRealTaxonId
            ? taxons[_random.Next(taxons.Count)].TaxonId
            : "does-not-exist";



        return (new Prop(author, id, TaxonId), useValidID, useRealTaxonId);
    }

    // Generates a random point in time, roughly between Jan 2000 and right now,
    // expressed as Unix seconds (matching how Cheep.Timestamp is stored/used elsewhere).

    private long RandomTimeStamp()
    {
        long minUnix = 946684800;
        long maxUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return (long)(minUnix + _random.NextDouble() * (maxUnix - minUnix));
    }

    //What the fuzztest going on here
    //Implementing Theory and InlineData 
    //The inLineData is used with attached seeds instead of a clean string or int, this makes it possible
    //to add the randomization to the process instead of just testing a specific string or int
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    
    public async Task FuzzObservationsAndComments_ServerStateMatchesOracle(int seed)
    {
        _random = new Random(seed);
            
        const int iterations = 50;

        for (int i = 0; i < iterations; i++)
        {
            // Bias slightly toward posting observations first (or always, if we have
            // none yet), so there's actually a pool of real IDs for comments to
            // reference - otherwise the first several rounds would have nothing
            // valid to pick from in GenerateRandomComment().

            bool postObservations = _random.NextDouble() < 0.5 || _sentObservations.Count == 0;

            if (postObservations)
            {
                var observation = GenerateRandomObservation();

                var response = await _client.PostAsJsonAsync("/observation", observation);
                response.EnsureSuccessStatusCode();

                // We record the SERVER's returned version (with its own assigned ID),
                // not our local placeholder one - the server's ID is what matters
                // when we check GET /observations later.

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

                response.EnsureSuccessStatusCode(); //This needs to be changed now that comments validate id's correctly

                _sentComments.Add(comment);
            }
            //ORACLE Check - with get function


        }



        // Ask the server for everything it currently has, and confirm every
        // observation we successfully posted is actually present, unmodified.

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

