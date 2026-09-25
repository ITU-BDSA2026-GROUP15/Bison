using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string _dbPath;

    public DBFacade(string dbPath)
    {
        _dbPath = dbPath;
    }

    public List<ObservationViewModel> GetObservations()
    {
        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand(); // det her viser bare alle observationer så at sige
        command.CommandText = @"
            select user.username, observation.text, observation.pub_date
            from observation
            join user on observation.author_id = user.user_id
            order by pub_date desc;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var author = reader.GetString(...);
            var message = reader.GetString(...);
            var timestamp = reader.GetInt64(...);

            result.Add(ObservationViewModel(author, message, timestamp));

        return result;
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
    {
        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand(); // det her viser alle observationer fra den bestemte author
        command.CommandText = @"
            select user.username, observation.text, observation.pub_date
            from observation
            join user on observation.author_id = user.user_id
            where user.username = @author
            order by observation.pub_date desc;";

        command.Parameters.AddWithValue("@author", author);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            //her skal der læses de tre kolloner
        }

        return result;
    }
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp) // copied from BisonService.cs
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
