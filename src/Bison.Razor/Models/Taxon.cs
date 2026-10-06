namespace Bison.Razor.Models;

// Represents a taxon with its identifiers, Danish name, and place in the taxonomy.
public class Taxon {
    // Internal database identifier.
    public int TaxonId { get; set; }
    // I did not know but it is "Darwin Core identifier" for this taxon.
    public string dwc_TaxonID { get; set; } = string.Empty;
    public string VernacularName { get; set; } = string.Empty;

    // Roden i taxon-træet har ingen forælder.
    // måske rettet på et tidspunkt?!
    //Hvis alle taxoner i  joined.csv har et parent-ID, må den øverste forælder ligge uden for datasættet.
    public int? ParentId { get; set; }

    public Taxon? Parent { get; set; }

    // Contains the immediate child taxa.
    public ICollection<Taxon> Children { get; set; } = new List<Taxon>();
}
