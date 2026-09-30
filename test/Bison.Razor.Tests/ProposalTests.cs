using Bison.Taxonomy;
using SimpleDB;

public class ProposalTests
{
    private const string ValidTaxonId = "MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea";

    [Fact]
    public void ExistingObservationIsAccepted()
    {
        var observations = new[]
        {
            new Cheep("Anne", 7, "A bird", 0, "ITU")
        };

        Assert.True(ProposalValidator.ObservationExists(observations, 7));
    }

    [Fact]
    public void MissingObservationIsRejected()
    {
        var observations = new[]
        {
            new Cheep("Anne", 7, "A bird", 0, "ITU")
        };

        Assert.False(ProposalValidator.ObservationExists(observations, 9999));
    }

    [Fact]
    public void ExistingTaxonIsAccepted()
    {
        var taxonomy = new Taxonomy();

        Assert.True(ProposalValidator.TaxonExists(taxonomy, ValidTaxonId));
    }

    [Fact]
    public void MissingTaxonIsRejected()
    {
        var taxonomy = new Taxonomy();

        Assert.False(ProposalValidator.TaxonExists(taxonomy, "not-a-taxon-id"));
    }
}
