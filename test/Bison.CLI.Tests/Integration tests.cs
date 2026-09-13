using SimpleDB;

namespace Bison.CLI.Tests;

public class IntegrationTest
{
    private const string ObserveFile = "integrationTest.csv";
    [Fact]
    public void DatabaseStoresAndReadsCorrectly()
    {
        // ARRANGE
        string testFilePath = ObserveFile;
        var db = CSVDatabase<Cheep>.Instance;
        var expectedCheep = new Cheep("testUser", 0, "Test observation", 1690891760);

        string? original = File.Exists(testFilePath) ? File.ReadAllText(testFilePath) : null;
        if (File.Exists(testFilePath))
            File.Delete(testFilePath);

        try
        {
            // ACT
            db.Store(testFilePath, expectedCheep);
            var resultater = db.Read(testFilePath).ToList();

            // ASSERT
            Assert.Single(resultater);
            Assert.Equal(expectedCheep, resultater[0]);
        }
        finally
        {
            // CLEANUP
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            if (original is not null)
                File.WriteAllText(ObserveFile, original);
        }
    }

    [Fact]
    public void nonExistingFileReturnsEmptyResultWhenRead()
    {
        // ARRANGE
        string nonExistingFilePath = "this_file_does_not_exist.csv";
        var db = CSVDatabase<Cheep>.Instance;

        if (File.Exists(nonExistingFilePath))
            File.Delete(nonExistingFilePath);

        // ACT
        var results = db.Read(nonExistingFilePath);

        // ASSERT
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}
