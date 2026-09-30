namespace Bison.Razor.Models;//meget vigtig!

// igen sub-klassen : super-klassen nedenunder sådan skrives det!
//ARVER fra post
public class Observation : Post {

    // Identifies the taxon associated with this observation.
    public int TaxonId { get; set; }
    // Reference to the taxon associated with this observation.
    public Taxon Taxon { get; set; } = null!;

    // Contains the comments on this observation.
    // Contains the proposed taxon identifications for this observation.
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}