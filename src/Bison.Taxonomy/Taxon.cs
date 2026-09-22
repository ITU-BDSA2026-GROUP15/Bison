namespace Bison.Taxonomy;

public record Taxon(string TaxonId, string? ParentId, string Rank, string ScientificName, string? VernacularName);
