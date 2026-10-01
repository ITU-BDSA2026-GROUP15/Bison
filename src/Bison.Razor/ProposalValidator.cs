using Bison.Razor.ObservationService;

public static class ProposalValidator
{
    public bool ObservationExists(int observationId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM observation WHERE id = $id LIMIT 1";
        command.Parameters.AddWithValue("$id", observationId);

        return command.ExecuteScalar() is not null;
    }

    // GetById returns null when the taxon ID does not exist in the taxonomy.
    // Kontrollerer, om det angivne taxon-ID findes i taksonomien.
    public static bool TaxonExists(Taxonomy taxonomy, string taxonId)
    {
         // Et resultat forskelligt fra null betyder, at taxonet findes.
        return taxonomy.GetById(taxonId) is not null;
    }
}
