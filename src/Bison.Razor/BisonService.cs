public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
    public List<ObservationViewModel> GetObservations();
    public List<ObservationViewModel> GetObservationsFromAuthor(string author);
}

public class ObservationService : IObservationService
{
    private readonly DBFacade _db;

    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations()
    {
        return _db.GetObservations(); // from dbfacade
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author)
    {
        return _db.GetObservationsFromAuthor(author);
    }

}
