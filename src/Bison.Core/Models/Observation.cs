namespace Bison.Core.Models;//meget vigtig!

// igen sub-klassen : super-klassen nedenunder sådan skrives det!
//ARVER fra post
public class Observation : Post {

    // Identifies the taxon associated with this observation.
    public int TaxonId { get; set; }
    // Reference to the taxon associated with this observation.
    public Taxon Taxon { get; set; } = null!;

    // Optional URL to a picture, if the user wants to add it to the observation. Null if not.
    public string? PictureUrl { get; set; }

    // Contains the comments on this observation.
    // Contains the proposed taxon identifications for this observation.
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
