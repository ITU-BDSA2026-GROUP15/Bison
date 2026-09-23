using Bison.Taxonomy;
using SimpleDB;

public static class ProposalValidator
{
    // Returns true when the proposal's observation ID matches an existing observation.
    public static bool ObservationExists(IEnumerable<Cheep> observations, int observationId)
    {
        return observations.Any(observation => observation.ID == observationId);
    }

    // GetById returns null when the taxon ID does not exist in the taxonomy.
    public static bool TaxonExists(Taxonomy taxonomy, string taxonId)
    {
        return taxonomy.GetById(taxonId) is not null;
    }
}
