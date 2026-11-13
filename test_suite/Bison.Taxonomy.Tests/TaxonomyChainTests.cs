
using Xunit;
using Bison.Taxonomy;

namespace Bison.Taxonomy.Tests;

public class TaxonomyChainTests
{
    [Fact]
    public void VerifyExampleChain_PelecaniformesToArdeaCinerea()
    {
        // Arrange
        var taxonomy = new Taxonomy();

        // Find arten
        var species = taxonomy.GetByVernacularName("Fiskehejre");

        Assert.NotNull(species);
        Assert.Equal("Ardea cinerea", species.ScientificName);

        // Act - følg kæden op gennem forældrene
        var genus = taxonomy.GetSupertaxon(species);
        var family = taxonomy.GetSupertaxon(genus!);
        var order = taxonomy.GetSupertaxon(family!);

        // Assert - kontrollér hele kæden
        Assert.NotNull(genus);
        Assert.NotNull(family);
        Assert.NotNull(order);

        Assert.Equal("Ardea", genus.ScientificName);
        Assert.Equal("Ardeidae", family.ScientificName);
        Assert.Equal("Pelecaniformes", order.ScientificName);
    }
}
