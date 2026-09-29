// Indeholder de oplysninger om en observation, som vises på siden.
public record ObservationViewModel(
    string Author, string Message, string Timestamp);

public interface IObservationService
{
    // Henter en side med observationer. Standard er side 1.
    public List<ObservationViewModel> GetObservations(int page = 1);

    // Henter en side med observationer fra en bestemt forfatter.
    public List<ObservationViewModel> GetObservationsFromAuthor(
        string author, int page = 1);

    public List<ObservationViewModel> GetObservationDetails (
        int id, int page = 1);

    public List<ObservationViewModel> GetProposals (int id, int page =1);

    public List<ObservationViewModel> GetComments (int id, int page =1);

}

public class ObservationService : IObservationService
{
    // DBFacade håndterer adgangen til SQLite-databasen.
    private readonly DBFacade _db;

    // Modtager DBFacade gennem dependency injection.
    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        // Sender sidetallet videre til databasekoden.
        return _db.GetObservations(page);
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(
        string author, int page = 1)
    {
        // Sender både forfatter og sidetal videre til databasekoden.
        return _db.GetObservationsFromAuthor(author, page);
    }

    public List<ObservationViewModel> GetObservationDetails (
        int id, int page = 1)
    {
        return _db.GetObservationDetails(id, page);
    }

     public List<ObservationViewModel> GetProposals (
        int id, int page = 1)
    {
        return _db.GetProposals(id, page);
    }

     public List<ObservationViewModel> GetComments (
        int id, int page = 1)
    {
        return _db.GetObservationDetails(id, page);
    }
}