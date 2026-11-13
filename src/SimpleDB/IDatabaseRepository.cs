namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    IEnumerable<Cheep> GetObservations();
    IEnumerable<Cheep> GetComments(int observationId);
    IEnumerable<Cheep> GetObservationsByLocation(string location);
    bool ObservationExists(int id);
    Cheep AddObservation(Cheep observation);
    void AddComment(Cheep comment);

}
