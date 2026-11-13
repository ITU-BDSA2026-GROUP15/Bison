using SimpleDB;
using System.Net.Http.Json;
using static UserInterface;
using Bison.CLI;
using Microsoft.VisualBasic;
using NSubstitute;

namespace Bison.CLI.Tests;

// [Collection(...)] sørger for, at CSVDBServiceFixture kun startes én gang og deles mellem
// alle tests i denne klasse, i stedet for at starte/stoppe web servicen for hver test.
[Collection("Webservice")]
public class UnitTest
{
    private readonly HttpClient client;
    private readonly Program program = new Program(Substitute.For<IDatabaseRepository>());

    public UnitTest(CSVDBServiceFixture fixture)
    {
        client = fixture.Client;
    }

    [Fact]
    public void ConvertTimeTest()
    {
        // ARRANGE
        long knownUnixTimestamp = 1690891760;
        DateTimeOffset expectedDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(knownUnixTimestamp).ToLocalTime();

        // ACT
        var actualResult = convertTime(knownUnixTimestamp);

        // ASSERT
        Assert.Equal(expectedDateTimeOffset, actualResult);
    }

    [Fact]
    public void NonexistingObservationTest()
    {
        // ARRANGE
        // Vi spørger web servicen om det nuværende højeste ID, i stedet for at bruge GetIdTracker()
        var observations = client.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult() ?? new List<Cheep>();
        int highestExistingId = observations.Any() ? observations.Max(o => o.ID) : -1;
        int nonExistingId = highestExistingId + 1000; // et ID der garanteret ikke findes
        string commentText = "This should not be stored";

        // Vi gemmer den originale TextWriter (almen terminal output) før vi overskriver den med en ny TextWriter.
        // Efter testen sætter vi den tilbage
        var originalOut = Console.Out;
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // ACT
            program.comment(nonExistingId, commentText);

            // ASSERT
            string output = consoleOutput.ToString();
            Assert.Contains("No observations with ID", output);
        }
        finally
        {
            // CLEANUP
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void ObserveIncrementsIdTrackerTest()
    {
        // ARRANGE
        // idBefore regnes nu ud fra web servicens data (samme logik som servicen selv bruger
        // til at tildele ID'er), i stedet for den lokale GetIdTracker()-variabel.
        var before = client.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult() ?? new List<Cheep>();
        int idBefore = before.Any() ? before.Max(o => o.ID) + 1 : 0;

        // Guid i observationsteksten sikrer, at vi finder netop vores egne observationer bagefter,
        // selvom CSV-filen indeholder data fra tidligere testkørsler.
        string firstObservation = "First test observation " + Guid.NewGuid();
        string secondObservation = "Second test observation " + Guid.NewGuid();

        // ACT
        program.observe(firstObservation, "ITU");
        program.observe(secondObservation, "ITU");

        // ASSERT
        var after = client.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult() ?? new List<Cheep>();
        var first = after.Single(o => o.Observation == firstObservation);
        var second = after.Single(o => o.Observation == secondObservation);

        Assert.Equal(idBefore, first.ID);
        Assert.Equal(idBefore + 1, second.ID);
    }
}
