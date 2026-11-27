using System.Net;
using System.Linq;
using Bison.Infrastructure.Repositories;
using Bison.Core.Models;
using Bison.Core.ModelsDTO;

// Indeholder de oplysninger om en observation, som vises på siden.
//translates between Repo and ObservationViewModel
public record ObservationViewModel(string Author, string Message, string Timestamp, string? PictureUrl = null, string? License = null);

/*{
    public int Id{get;init;}//init; betyder, at værdien må sættes, når objektet oprettes.
}*/

public interface IObservationService
{
    // Henter en side med observationer. Standard er side 1.

    List<ObservationDto> GetObservations (int page = 1);

    // Henter en side med observationer fra en bestemt forfatter.
    List<ObservationDto> GetObservationsFromAuthor(
        string author, int page = 1);



    ObservationDto? GetObservationDetails (
        int id);

    List<ProposalDto> GetProposals (int id, int page =1);

    List<CommentDto> GetComments (int id, int page =1);
    
    
    IReadOnlyList<Taxon> GetTaxons();

    // Forsøger at gemme en observation for den angivne forfatter.
    // E-mailen bruges, hvis forfatteren skal oprettes.
    // taxonId identificerer det taxon, brugeren har valgt.
    // Returnerer true, når observationen er gemt, og false,
    // hvis forfatternavnet eller teksten mangler, taxonet ikke findes,
    // eller der er et billede uden licens.
    bool TryCreateObservation(
        string authorName, string email, string text, int taxonId,  string? pictureUrl, string? license);

    bool TryCreateComment(
        string authorName, string email, string text, int observationID);

    bool TryCreateProposal(
        string authorName, string email, string text, int observationId, int taxonId);

}

public class ObservationService : IObservationService
{
    
    private readonly IPostRepository _repo;

    
    public ObservationService(IPostRepository repo)
    {
        _repo = repo;
    }

    public List<ObservationDto> GetObservations(int page = 1)
    
        // Sender sidetallet videre til databasekoden.
        => _repo.GetObservations(page).ToList();
    

  public List<ObservationDto> GetObservationsFromAuthor(string author, int page=1)
    
        => _repo.GetObservationsByAuthor(author,page).ToList();
    
// now the getObservationDetails have and the entity -> viewModel conversion
       public ObservationDto? GetObservationDetails (int id)
    
      => _repo.GetObservationDetails(id);

    

     public List<ProposalDto> GetProposals (int id, int page = 1)

        => _repo.GetProposals(id).ToList();


     public List<CommentDto> GetComments (int id, int page = 1)

        => _repo.GetComments(id).ToList();


    public IReadOnlyList<Taxon> GetTaxons()
    {
         // Hent de taxoner, som brugeren kan vælge i observationsformularen.
        // Repositoryet står for at læse dem fra databasen.
        return _repo.GetTaxons();
    }


    public bool TryCreateObservation(
        string authorName, string email, string text, int taxonId, string? pictureUrl, string? license)
    {
        // En observation skal have en forfatter og en tekst.
        // IsNullOrWhiteSpace fanger også tekst, der kun består af mellemrum.
        if (string.IsNullOrWhiteSpace(authorName) ||
            string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        // Et billede skal have en licens.
        if (pictureUrl != null && string.IsNullOrWhiteSpace(license))
            return false;

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
            PictureUrl = pictureUrl,
            License = license,

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
        var obs = _repo.GetObservationEntity(observationId);
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

        var obs = _repo.GetObservationEntity(observationId);
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
