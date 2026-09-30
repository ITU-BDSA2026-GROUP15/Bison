namespace Bison.Razor.Models;

// Represents a taxon with its identifiers, Danish name, and place in the taxonomy.
public class Taxon {
    // Internal database identifier.
    public int Id { get; set; }
    // I did not know but it is "Darwin Core identifier" for this taxon.
    public string dwc_TaxonID { get; set; } = string.Empty;
    public string DanishVernacularName { get; set; } = string.Empty;

    
    public int? ParentId { get; set; }
    
    public Taxon? Parent { get; set; }
    
    // Contains the immediate child taxa.
    public ICollection<Taxon> Children { get; set; } = new List<Taxon>();
}