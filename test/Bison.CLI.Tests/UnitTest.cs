using SimpleDB;
using static UserInterface;
using static Program;

namespace Bison.CLI.Tests;

public class UnitTest
{
    private static string test_file = "testfile_empty.csv";
    private CSVDatabase<string> TestDb { get; } = new(test_file);

    [Fact]
    public void ConvertTimeTest()
    {
        // ARRANGE
        long knownUnixTimestamp = 1690891760;
        DateTimeOffset expectedDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(knownUnixTimestamp).ToLocalTime();

        // ACT
        var actualResult = convertTime(knownUnixTimestamp);

        // ASSERT
        Assert.Equal(actualResult, expectedDateTimeOffset);
    }

    [Fact]
    public void NonexistingObservationTest()
    {
        // ARRANGE
        int nonExistingId = GetIdTracker() + 1;
        string CommentText = "This should not be stored";

        // ACT

        comment(nonExistingId, CommentText);

        // ASSERT
        ASSERT databasen (CSV-filen) er stadig tom / indeholder IKKE commentText
        ASSERT konsol-output indeholder fejlbesked om at ID ikke findes
    }




}
