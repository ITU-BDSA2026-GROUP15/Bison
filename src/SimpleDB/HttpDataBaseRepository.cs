
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
        
    }

    IEnumerable<Cheep> GetComments(int observationId)
    {
        
    }
    IEnumerable<Cheep> GetObservationsByLocation(string location)
    {
        
    }
    bool ObservationExists(int id)
    {
            
    }
    Cheep AddObservation(Cheep observation)
    {
        
    }
    void AddComment(Cheep comment){
        
    }
}