using System.Reflection;

public class ProposalsUnitTests
{
    [Fact]
    public void TryCreateProposalFromValidInput()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};
        
        // create an observation we can test proposals on
        var o = new Observation
        {
            Id = 1, 
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author);
        var service = new ObservationService(repository);

        // act
        // try to create a proposal based on the observation above
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.ID, taxon.TaxonId);

        // assert
        // expect the service to return true and send a proposal
        // also expect the repo to call Save()

        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(proposal,"Proposal was created successfully");
        proposal = Assert.IsType<Proposal>(repository.InsertedPost);
        Assert.Same(taxon, proposal.Taxon);
        Assert.Same(author, proposal.Author);
        Assert.Same(o, proposal.Observation);
        Assert.Equal("PROPOSAL TEST", proposal.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalFromUnknownTaxon()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalFromUnknownObservation()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalWithEmptyAuthorName()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalWithWhitespaceText()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalWithInvalidObservationId()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalWithInvalidTaxonId()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalReusesExistingAuthor()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalCreatesNewAuthorWhenUnknown()
    {
        // arrange

        // act

        // assert
    }

    [Fact]
    public void TryCreateProposalTrimsText()
    {
        // arrange

        // act

        // assert
    }
}