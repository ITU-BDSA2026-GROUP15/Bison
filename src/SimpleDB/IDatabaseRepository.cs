
namespace SimpleDB;

public interface IDatabaseRepository
{
    public IEnumerable<Cheep> GetObservations();
    public IEnumerable<Cheep> GetComments(int observationId);
    public IEnumerable<Cheep> GetObservationsByLocation(string location);
    public bool ObservationExists(int id);
    public Cheep AddObservation(Cheep observation);
    public Cheep AddComment(Cheep comment);

}
