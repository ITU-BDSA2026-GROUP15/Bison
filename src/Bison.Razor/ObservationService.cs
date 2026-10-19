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

    public List<ObservationViewModel> GetObservations (int page = 1);

    // Henter en side med observationer fra en bestemt forfatter.
    public List<ObservationViewModel> GetObservationsFromAuthor(
        string author, int page = 1);

    public List<ObservationViewModel> GetObservationDetails (
        int id, int page = 1);

    public List<ObservationViewModel> GetProposals (int id, int page =1);

    public List<ObservationViewModel> GetComments (int id, int page =1);

    public List<ObservationViewModel> GetObservationsFromTaxon (DafnyTaxon root, int page = 1);
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
        return _repo.GetPosts<Observation>(page).Select(ToViewModel).ToList();
    }

  public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page=1)
    {
        return _repo.GetPostsByAuthor<Observation>(author, page).Select(ToViewModel).ToList();
    }
// now the getObservationDetails have and the entity -> viewModel conversion
    public List<ObservationViewModel> GetObservationDetails (int id, int page){

        var observation = _repo.GetObservationDetails(id);
        return observation is null
            ? new List<ObservationViewModel>()
            : new List<ObservationViewModel> { ToViewModel(observation) };

    }

     public List<ObservationViewModel> GetProposals (
        int id, int page = 1)
    {
        return _repo.GetProposals(id).Select(ToProposalViewModel).ToList();
    }

     public List<ObservationViewModel> GetComments (
        int id, int page = 1)
    {
        return _repo.GetComments(id).Select(ToCommentViewModel).ToList();
    }

    public List<ObservationViewModel> GetObservationsFromTaxon(DafnyTaxon root, int page = 1)
    {
        //wrapping an observation to a DafnyObservation
        var dafnyObs = _repo.GetPosts<Observation>(page).Select(obs => new DafnyObservation(obs)).ToArray();

        //converting to a dafny-sequence, which is the parameter for the wrapper class
        var input = Dafny.Sequence<DafnyObservation>.FromArray(dafnyObs);
        
        //calling the wrapper, and verify the filtering
        var result = DafnyWrapper.Wrap.Filter(root, input);

        //unwrapping from dafnyObservation to Observation and convert to ViewModel
        return result.Elements.Select(daf => ToViewModel(daf.Observation)).ToList();
    }
    private static ObservationViewModel ToViewModel(Observation o)
    {
        return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
    }
	private static ObservationViewModel ToProposalViewModel(Proposal o)
	{
		return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
	}

	private static ObservationViewModel ToCommentViewModel(Comment o)
	{
		return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
	}


}
