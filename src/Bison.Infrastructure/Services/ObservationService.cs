using Bison.Core.Models;
using Bison.Infrastructure.Repositories;

namespace Bison.Infrastructure.Services;

// Indeholder de oplysninger om en observation, som vises på siden.
//translates between Repo and ObservationViewModel
public record ObservationViewModel(string Author, string Message, string Timestamp, string? PictureUrl = null);

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

    // Henter de taxoner, brugeren kan vælge i observationsformularen.
    IReadOnlyList<Taxon> GetTaxons();

    // Forsøger at gemme en observation for den angivne forfatter.
    // E-mailen bruges, hvis forfatteren skal oprettes.
    // taxonId identificerer det taxon, brugeren har valgt.
    // Returnerer true, når observationen er gemt, og false,
    // hvis forfatternavnet eller teksten mangler, eller taxonet ikke findes.
    bool TryCreateObservation(
        string authorName, string email, string text, int taxonId);

    bool TryCreateComment(
        string authorName, string email, string text, int observationID);

    bool TryCreateProposal(
        string authorName, string email, string text, int observationId, int taxonId);

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

    public static ObservationViewModel ToViewModel(Observation o)
    {
        return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"), o.PictureUrl);
    }
    private static ObservationViewModel ToProposalViewModel(Proposal o)
    {
        return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
    }

    private static ObservationViewModel ToCommentViewModel(Comment o)
    {
        return new ObservationViewModel(o.Author.Name, o.Text, o.TimeStamp.ToString("g"));
    }


    public IReadOnlyList<Taxon> GetTaxons()
    {
        // Hent de taxoner, som brugeren kan vælge i observationsformularen.
        // Repositoryet står for at læse dem fra databasen.
        return _repo.GetTaxons();
    }

    public bool TryCreateObservation(
        string authorName, string email, string text, int taxonId)
    {
        // En observation skal have en forfatter og en tekst.
        // IsNullOrWhiteSpace fanger også tekst, der kun består af mellemrum.
        if (string.IsNullOrWhiteSpace(authorName) ||
            string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        // Slå det valgte taxon op. Et indsendt ID kan være ugyldigt,
        // selvom brugeren normalt vælger det fra en dropdown.
        var taxon = _repo.GetTaxon(taxonId);
        if (taxon is null)
            return false;

        // Genbrug forfatteren, hvis vedkommende allerede findes.
        // Operatoren ?? opretter kun en ny Author, når opslaget giver null.
        // E-mailen bruges til at udfylde den nye forfatters oplysninger.
        var author = _repo.GetAuthorByName(authorName) ?? new Author
        {
            Name = authorName,
            Email = email
        };

        // Knyt observationen til forfatteren og det valgte taxon.
        var observation = new Observation
        {
            Author = author,
            Taxon = taxon,

            // Fjern mellemrum før og efter teksten.
            Text = text.Trim(),

            // Brug serverens aktuelle tid i UTC som oprettelsestidspunkt.
            TimeStamp = DateTime.UtcNow
        };

        // InsertPost registrerer observationen til indsættelse.
        // Save skriver ændringerne til databasen. Hvis Author er ny,
        // bliver den gemt sammen med observationen.
        _repo.InsertPost(observation);
        _repo.Save();

        // true betyder, at gemningen blev gennemført.
        return true;
    }

    public bool TryCreateComment(
        string authorName, string email, string text, int observationId)
    {
        // En kommentar skal have en forfatter, en tekst og et id der knytter den til en obs.
        // IsNullOrWhiteSpace fanger også tekst, der kun består af mellemrum.
        if (string.IsNullOrWhiteSpace(authorName) ||
            string.IsNullOrWhiteSpace(text) || observationId <= 0) // 0 er svarende til null for int
        {
            return false;
        }

        // Slå den tilknytted observation op. Et indsendt ID kan være ugyldigt,
        // selvom brugeren normalt vælger det fra en dropdown.
        var obs = _repo.GetObservationDetails(observationId);
        if (obs is null)
            return false;

        // Genbrug forfatteren, hvis vedkommende allerede findes.
        // Operatoren ?? opretter kun en ny Author, når opslaget giver null.
        // E-mailen bruges til at udfylde den nye forfatters oplysninger.
        var author = _repo.GetAuthorByName(authorName) ?? new Author
        {
            Name = authorName,
            Email = email
        };

        // Knyt kommentaren til forfatteren og den valgte observation.
        var comment = new Comment
        {
            Author = author,
            Observation = obs,

            // Fjern mellemrum før og efter teksten.
            Text = text.Trim(),

            // Brug serverens aktuelle tid i UTC som oprettelsestidspunkt.
            TimeStamp = DateTime.UtcNow
        };

        // InsertPost registrerer kommentaren til indsættelse.
        // Save skriver ændringerne til databasen. Hvis Author er ny,
        // bliver den gemt sammen med observationen.
        _repo.InsertPost(comment);
        _repo.Save();

        // true betyder, at gemningen blev gennemført.
        return true;
    }

    public bool TryCreateProposal(
        string authorName, string email, string text, int observationId, int taxonId)
    {
        // En proposal skal have en forfatter, et id der knytter den til en obs, samt et taxon id.
        // IsNullOrWhiteSpace fanger også tekst, der kun består af mellemrum.
        if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(text) ||
            observationId <= 0 || taxonId <= 0) // 0 er svarende til null for int
        {
            return false;
        }

        // Slå den tilknytted proposal op. Et indsendt ID kan være ugyldigt,
        // selvom brugeren normalt vælger det fra en dropdown.
        var taxon = _repo.GetTaxon(taxonId);
        if (taxon is null)
            return false;

        var obs = _repo.GetObservationDetails(observationId);
        if (obs is null)
            return false;

        // Genbrug forfatteren, hvis vedkommende allerede findes.
        // Operatoren ?? opretter kun en ny Author, når opslaget giver null.
        // E-mailen bruges til at udfylde den nye forfatters oplysninger.
        var author = _repo.GetAuthorByName(authorName) ?? new Author
        {
            Name = authorName,
            Email = email
        };

        // Knyt proposal til forfatteren og den valgte observation.
        var proposal = new Proposal
        {
            Author = author,
            Observation = obs,
            ObservationId = observationId,

            // Fjern mellemrum før og efter teksten.
            Taxon = taxon,
            Text = text.Trim(),

            // Brug serverens aktuelle tid i UTC som oprettelsestidspunkt.
            TimeStamp = DateTime.UtcNow
        };

        // InsertPost registrerer proposal til indsættelse.
        // Save skriver ændringerne til databasen. Hvis Author er ny,
        // bliver den gemt sammen med proposal.
        _repo.InsertPost(proposal);
        _repo.Save();

        // true betyder, at gemningen blev gennemført.
        return true;
    }

}