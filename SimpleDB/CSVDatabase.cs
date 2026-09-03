namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;

    public CSVDatabase(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        // read CSV here with CsvHelper
    }

    public void Store(T record)
    {
        // write CSV here with CsvHelper
    }
}