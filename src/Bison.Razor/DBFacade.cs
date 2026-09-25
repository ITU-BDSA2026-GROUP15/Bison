using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string _dbPath;

    public DBFacade(string dbPath)
    {
        //her skal stien gemmes i feltet
    }

    public List<ObservationViewModel> GetObservations()
    {
        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"TODO: din forespørgsel A";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            //her skal der læses de tre kolloner
        }

        return result;
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
    {
        // forespørgsel B

        return
    }
}
