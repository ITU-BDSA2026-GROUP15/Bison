using Bison.Taxonomy;
using SimpleDB;

public static class ProposalValidator
{
    // Returns true when the proposal's observation ID matches an existing observation.
    // Kontrollerer, om der findes en observation med det angivne ID.
    public static bool ObservationExists(IEnumerable<Cheep> observations, int observationId)
    {   //what is the meaning of =>!
        // Lambdaen sammenligner hver observations ID med observationId.
        // Any returnerer true, så snart en observation matcher.
        return observations.Any(observation => observation.ID == observationId);
    }

    // GetById returns null when the taxon ID does not exist in the taxonomy.
    // Kontrollerer, om det angivne taxon-ID findes i taksonomien.
    public static bool TaxonExists(Taxonomy taxonomy, string taxonId)
    {
         // Et resultat forskelligt fra null betyder, at taxonet findes.
        return taxonomy.GetById(taxonId) is not null;
    }
}
