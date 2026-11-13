using SimpleDB;
using System.Net.Http.Json;
using Microsoft.VisualBasic;
using NSubstitute;

namespace Bison.CLI.Tests;

public class UnitTest
{
    private readonly IDatabaseRepository _db = Substitute.For<IDatabaseRepository>();
    private readonly Program _program;

    public UnitTest()
    {
        _program = new Program(_db);
    }

    [Fact]
    public void ConvertTimeTest()
    {
        // ARRANGE
        long knownUnixTimestamp = 1690891760;
        var expected = DateTimeOffset.FromUnixTimeSeconds(knownUnixTimestamp).ToLocalTime();

        // ACT
        var actual = convertTime(knownUnixTimestamp);

        // ASSERT
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NonexistingObservationTest()
    {
        // ARRANGE
        _db.ObservationExists(999).Returns(false);


        // Vi gemmer den originale TextWriter (almen terminal output) før vi overskriver den med en ny TextWriter.
        // Efter testen sætter vi den tilbage
        var originalOut = Console.Out;
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // ACT
            _program.comment(999, "This should not be stored");

            // ASSERT
            string output = consoleOutput.ToString();
            Assert.Contains("No observations with ID", output);

            _db.DidNotReceive().AddComment(Arg.Any<Cheep>());
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
        var stored = new List<Cheep>();

        _db.AddObservation(Arg.Any<Cheep>()).Returns(ci =>
        {
            var c = ci.Arg<Cheep>();
            int nextId = stored.Any() ? stored.Max(o => o.ID) + 1 : 0;
            var withId = new Cheep(c.Author, nextId, c.Observation, c.Timestamp, c.Location);
            stored.Add(withId);
            return withId;
        });

        int idBefore = stored.Any() ? stored.Max(o => o.ID) + 1 : 0;

        // Guid i observationsteksten sikrer, at vi finder netop vores egne observationer bagefter,
        // selvom CSV-filen indeholder data fra tidligere testkørsler.
        string firstObservation = "First test observation " + Guid.NewGuid();
        string secondObservation = "Second test observation " + Guid.NewGuid();

        // ACT
        program.observe(firstObservation, "ITU");
        program.observe(secondObservation, "ITU");

        // ASSERT
        var first = stored.Single(o => o.Observation == firstObservation);
        var second = stored.Single(o => o.Observation == secondObservation);

        Assert.Equal(idBefore, first.ID);
        Assert.Equal(idBefore + 1, second.ID);
    }
}
