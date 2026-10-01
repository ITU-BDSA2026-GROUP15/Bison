using System.Globalization;

public class DBFacade
{
    private readonly string _dbPath;

    public DBFacade(string dbPath)
    {
        _dbPath = dbPath;
    }

    public List<ObservationViewModel> GetObservations(int page = 1)
    {

        // Sidetallet er mindst 1. Hver side indeholder højst 32 observationer.
        page = Math.Max(1, page);
        long offset = ((long)page - 1) * 32;

        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand(); // det her viser bare alle observationer så at sige

       // Før hentede SQL'en alle observationer sorteret efter dato.
        // Nu henter den højst 32 og springer tidligere sider over med OFFSET.
        // Id bruges som ekstra sortering, når observationer har samme dato.

       command.CommandText = @"
            select user.username, observation.text, observation.pub_date
            from observation
            join user on observation.author_id = user.user_id
            order by observation.pub_date desc, observation.observation_id desc
            limit 32 offset @offset;";

        // Springer observationer fra tidligere sider over direkte i databasen.
        command.Parameters.AddWithValue("@offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var author = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);

            var timestampString = UnixTimeStampToDateTimeString(timestamp);

            result.Add(new ObservationViewModel(author, message, timestampString));
        }

        return result;
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 1)
    {
        // Samme tilføjelse.
        page = Math.Max(1, page);
        long offset = ((long)page - 1) * 32;


        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand(); // det her viser alle observationer fra den bestemte author

        // Før hentede SQL'en alle observationer fra den valgte forfatter.
        // Nu filtrerer den stadig efter forfatter, men henter kun den ønskede
        // side med højst 32 observationer ved hjælp af LIMIT og OFFSET.
        // Id sikrer en fast rækkefølge, når observationer har samme dato.


        command.CommandText = @"
            select user.username, observation.text, observation.pub_date
            from observation
            join user on observation.author_id = user.user_id
            where user.username = @author
            order by observation.pub_date desc, observation.observation_id desc
            limit 32 offset @offset;";

        command.Parameters.AddWithValue("@author", author);

        // Springer observationer fra tidligere sider over direkte i databasen.
        command.Parameters.AddWithValue("@offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var username = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);
            var timestampString = UnixTimeStampToDateTimeString(timestamp);

            result.Add(new ObservationViewModel(username, message, timestampString));
        }

        return result;
    }

    public List<ObservationViewModel> GetObservationDetails(int id, int page = 1)
    {
        // Samme tilføjelse.
        page = Math.Max(1, page);

        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();

        // Nu sortere vi efter efter id
        command.CommandText = @"
            select user.username, observation.text, observation.pub_date
            from observation
            join user
                on user.user_id = observation.author_id
            where observation.observation_id = @id;";


        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var username = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);
            var timestampString = UnixTimeStampToDateTimeString(timestamp);

            result.Add(new ObservationViewModel(username, message, timestampString));
        }

        return result;
    }

    public List<ObservationViewModel> GetProposals (int id, int page = 1)
    {
        // Samme tilføjelse.
        page = Math.Max(1, page);

        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();

        // Nu sortere vi efter efter id
        command.CommandText = @"
            select user.username, proposal.taxon_id, proposal.pub_date
            from proposal
            join observation
                on observation.observation_id = proposal.proposal_id
            join user
                on user.user_id = observation.author_id
            where proposal.proposal_id = @id;";

        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var author = reader.GetString(0);
            var taxon_id = reader.GetString(1);
            var timestamp = reader.GetInt64(2);
            var timestampString = UnixTimeStampToDateTimeString(timestamp);

            result.Add(new ObservationViewModel(author, taxon_id, timestampString));
        }

        return result;
    }

    public List<ObservationViewModel> GetComments (int id, int page = 1)
    {
        // Samme tilføjelse.
        page = Math.Max(1, page);

        var result = new List<ObservationViewModel>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();

        // Nu sortere vi efter efter id
        command.CommandText = @"
            select user.username, comment.text, comment.pub_date
            from comment
            join observation
                on observation.observation_id = comment.comment_id
            join user
                on user.user_id = observation.author_id
            where comment.comment_id = @id;";

        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var author = reader.GetString(0);
            var comment = reader.GetString(1);
            var timestamp = reader.GetInt64(2);
            var timestampString = UnixTimeStampToDateTimeString(timestamp);

            result.Add(new ObservationViewModel(author, comment, timestampString));
        }

        return result;
    }

     public bool ObservationExists(int observationId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM observation WHERE id = $id LIMIT 1";
        command.Parameters.AddWithValue("$id", observationId);

        return command.ExecuteScalar() is not null;
    }

    // public, så den kan unit-testes fra Bison.Razor.Tests.

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp) // copied from BisonService.cs
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);

        // InvariantCulture: '/' og ':' i formatet betyder "kulturens separator", så uden den ville
        // en dansk maskine vise 08-01-23 i stedet for 08/01/23. Nu er formatet ens på alle maskiner.
        return dateTime.ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture);
    }
}
