using SimpleDB;

namespace SimpleDB.Tests;

public record TestRecord(string Author, int Id, string Observation, long Timestamp);

public class CsvDatabaseIntegrationTests
{
    [Fact]
    public void StoreThenRead_GivenSingleRecord_ReturnsSameRecord()
    {
        // ARRANGE
        string testFilePath = Path.Combine(Path.GetTempPath(), $"csvdb_test_{Guid.NewGuid()}.csv");
        var db = CSVDatabase<TestRecord>.Instance;
        var expected = new TestRecord("testUser", 0, "Test observation", 1690891760);

        try
        {
            // ACT
            db.Store(testFilePath, expected);
            var results = db.Read(testFilePath).ToList();

            // ASSERT
            Assert.Single(results);
            Assert.Equal(expected, results[0]);
        }
        finally
        {
            // CLEANUP
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }

    // REGRESSION: Store() used to only ever write the data row, never a CSV header.
    // That worked "by accident" only because the committed .csv files already had a
    // header line typed in manually. Storing into any brand-new file produced a
    // headerless CSV that CsvHelper's Read() could never parse back (see the
    // "Header with name 'Author' was not found" failure this test used to reproduce).
    // This test pins that Store() now writes the header itself for a fresh file.
    [Fact]
    public void Store_GivenBrandNewFile_WritesHeaderRowBeforeRecord()
    {
        // ARRANGE
        string testFilePath = Path.Combine(Path.GetTempPath(), $"csvdb_test_{Guid.NewGuid()}.csv");
        var db = CSVDatabase<TestRecord>.Instance;
        var record = new TestRecord("testUser", 0, "Test observation", 1690891760);
        Assert.False(File.Exists(testFilePath));

        try
        {
            // ACT
            db.Store(testFilePath, record);
            string[] lines = File.ReadAllLines(testFilePath);

            // ASSERT
            Assert.Equal(2, lines.Length);
            Assert.Equal("Author,Id,Observation,Timestamp", lines[0]);
            Assert.Equal("testUser,0,Test observation,1690891760", lines[1]);
        }
        finally
        {
            // CLEANUP
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }

    [Fact]
    public void Read_GivenNonExistingFile_ReturnsEmptyResult()
    {
        // ARRANGE
        string nonExistingFilePath = Path.Combine(Path.GetTempPath(), $"does_not_exist_{Guid.NewGuid()}.csv");
        var db = CSVDatabase<TestRecord>.Instance;
        Assert.False(File.Exists(nonExistingFilePath));

        // ACT
        var results = db.Read(nonExistingFilePath);

        // ASSERT
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}
