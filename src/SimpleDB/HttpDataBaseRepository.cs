
namespace SimpleDB;

public class HttpDatabaseRepository : IDatabaseRepository
{
    private readonly HttpClient _httpClient;


    public HttpDatabaseRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IEnumerable<Cheep> GetObservations()
    {
        //henter nu observationerne fra web servicen (GET /observations) i stedet for at læse CSV-filen direkte.
        var task = _httpClient.GetFromJsonAsync<List<Cheep>>("/observations");
        var awaiter = task.GetAwaiter();
        var cheeps = awaiter.GetResult();

        return UserInterface.PrintObservations(cheeps ?? new List<Cheep>());
    }

    IEnumerable<Cheep> GetComments(int observationId)
    {
        var postTask = _httpClient.PostAsJsonAsync($"/comment?id={observationId}");
        var postAwaiter = postTask.GetAwaiter();
        postAwaiter.GetResult();
        return UserInterface.PrintCommentAdded(cheep);
    }
    
    IEnumerable<Cheep> GetObservationsByLocation(string location)
    {
        var cheeps = GetObservations();
        var matches = cheeps.Where(cheep => string.Equals(cheep.Location, location, StringComparison.OrdinalIgnoreCase));

       return UserInterface.PrintObservations(matches);
    }
    bool ObservationExists(int id)
    {
        return GetObservations().Any(cheep => cheep.ID == id);
    }
    Cheep AddObservation(Cheep observation)
    {
        var postTask = _httpClient.PostAsJsonAsync("/observation", observation);
        var postAwaiter = postTask.GetAwaiter();
        var response = postAwaiter.GetResult();

        var readTask = response.Content.ReadFromJsonAsync<Cheep>();
        var readAwaiter = readTask.GetAwaiter();
        var storedCheep = readAwaiter.GetResult();

        return storedCheep ?? observation;
    }
    void AddComment(Cheep comment){

        var postTask = _httpClient.PostAsJsonAsync("/comment", comment);
        var postAwaiter = postTask.GetAwaiter();
        postAwaiter.GetResult();
    }
}