using SimpleDB;
using static UserInterface;
using static Program;

namespace Bison.CLI.Tests;

public class UnitTest
{

    private const string ObserveFile = "testfile_empty.csv";

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
        int nonExistingId = GetIdTracker() + 1;
        string commentText = "This should not be stored";

        // Linjen nedenunder gemmer CSV filen som den ser ud inden vi nulstiller den til testen

        // Gemmer CSV filen
        string? originalContent = File.Exists(ObserveFile) ? File.ReadAllText(ObserveFile) : null;
        // "Nulstiller" CSV filen
        File.WriteAllText(ObserveFile, string.Empty);

        // Vi gemmer den originale TextWriter (almen terminal output) før vi overskriver den med en ny TextWriter.
        // Efter testen sætter vi den tilbage
        var originalOut = Console.Out;
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // ACT
            comment(nonExistingId, commentText);

            // ASSERT
            string fileContentAfter = File.ReadAllText(ObserveFile);
            Assert.Equal(string.Empty, fileContentAfter);

            string output = consoleOutput.ToString();
            Assert.Contains("No observations with ID", output);
        }
        finally
        {
            // CLEANUP
            Console.SetOut(originalOut);

            if (originalContent is null)
                File.Delete(ObserveFile);
            else
                File.WriteAllText(ObserveFile, originalContent);
        }
    }



    [Fact]
    public void ObserveIncrementsIdTrackerTest()
    {
        // ARRANGE
        int idBefore = GetIdTracker();
        string firstObservation = "First test observation";
        string secondObservation = "Second test observation";

        string? originalContent = File.Exists(ObserveFile) ? File.ReadAllText(ObserveFile) : null;
        File.WriteAllText(ObserveFile, string.Empty);

        try
        {
            // ACT
            //NEEDS A LOCATION! UPDATED IN PROGRAM.CS
            observe(firstObservation);
            observe(secondObservation);

            // ASSERT
            var db = CSVDatabase<Cheep>.Instance;
            var cheeps = db.Read(ObserveFile).ToList();

            Assert.Equal(2, cheeps.Count);
            Assert.Equal(idBefore + 1, cheeps[0].ID);
            Assert.Equal(idBefore + 2, cheeps[1].ID);
        }
        finally
        {
            if (originalContent is null)
                File.Delete(ObserveFile);
            else
                File.WriteAllText(ObserveFile, originalContent);
        }
    }
}
