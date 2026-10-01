namespace Bison.Razor.Models;

// ARVER AL Fra POST!

//ENGELSK FOR AT VÆRE GO'
// Represents a proposed taxon identification and inherits common properties from Post.
public class Proposal : Post {
    // Identifies the observation this proposal belongs to.
    public int ObservationId { get; set; }
    // Reference to the observation this proposal belongs to.
    public Observation Observation { get; set; } = null!;

    // Identifies the proposed taxon.
    public int TaxonId { get; set; }
    // Reference to the proposed taxon.
    public Taxon Taxon { get; set; } = null!;
}