namespace Bison.Taxonomy;

/*
This is the data model for Taxon
*/
public record Taxon(string TaxonId, string? ParentId, string Rank, string ScientificName, string? VernacularName);
