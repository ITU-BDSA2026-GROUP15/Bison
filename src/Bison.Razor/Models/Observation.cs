namespace Bison.Razor.Models;

// igen sub-klassen : super-klassen nedenunder sådan skrives det!
public class Observation : Post {
    public int TaxonId { get; set; }
    public Taxon Taxon { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}