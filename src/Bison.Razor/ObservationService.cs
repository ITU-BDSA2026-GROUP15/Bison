using System.Net;
using System.Linq;
using Bison.Razor.DAL;
using Bison.Razor.Models;

// Indeholder de oplysninger om en observation, som vises på siden.
//translates between Repo and ObservationViewModel
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
    private readonly IPostRepository _repo;

    // Modtager DBFacade gennem dependency injection.
    public ObservationService(IPostRepository repo)
    {
        _repo = repo;
    }

    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        // Sender sidetallet videre til databasekoden.
        return _repo.GetPosts<Observation>(author, page).Select(ToViewModel).ToList();
    }

  public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page=1)
    {
        return _repo.GetPostsByAuthor<Observation>(author).Select(ToViewModel).ToList();
    }

    public List<ObservationViewModel> GetObservationDetails (
        int id, int page = 1)
    {
        return _repo.GetObservationDetails(id, page);
    }

     public List<ObservationViewModel> GetProposals (
        int id, int page = 1)
    {
        return _repo.GetProposals(id, page);
    }

     public List<ObservationViewModel> GetComments (
        int id, int page = 1)
    {
        return _repo.GetComments(id, page);
    }

    private static ObservationViewModel ToViewModel(Observation o)
    {
        return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
    }
}
