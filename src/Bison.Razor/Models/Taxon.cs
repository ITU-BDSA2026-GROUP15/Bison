namespace Bison.Razor.Models;

public class Taxon {
    public int Id { get; set; }
    public string dwc_TaxonID { get; set; } = string.Empty;
    public string DanishVernacularName { get; set; } = string.Empty;

    // Roden i taxon-træet har ingen forælder.
    public int? ParentId { get; set; }
    public Taxon? Parent { get; set; }
    public ICollection<Taxon> Children { get; set; } = new List<Taxon>();
}