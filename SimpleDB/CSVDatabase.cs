namespace SimpleDB;

using CsvHelper;
using System.Globalization;
using System.IO;
using System;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;

    public CSVDatabase(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        try {
        
        using (var reader=  new StreamReader("bison_observe_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
            
            var cheeps= csv.GetRecords<T>().ToList();
            
            // ??????
            return limit.HasValue ? cheeps.Take(limit.Value) : cheeps; //tilføj forklaring her :)
            }
        }

        catch (Exception e) {
            Console.WriteLine("File could not be read: ");
            Console.WriteLine(e.Message);
            
            return Enumerable.Empty<T>();
        }
    }
    
    public void Store(T record)
    {
        // write CSV here with CsvHelper
    }
}