namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? _instance;

    public static CSVDatabase<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CSVDatabase<T>();
            }
            return _instance;
        }
    }

    private CSVDatabase()
    {
    }

    public IEnumerable<T> Read(string file, int? limit = null)
    {
        try {

        using (var reader=  new StreamReader(file))


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

    public void Store(string file, T record)
    {
        using (var writer = new StreamWriter(file, true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

}
