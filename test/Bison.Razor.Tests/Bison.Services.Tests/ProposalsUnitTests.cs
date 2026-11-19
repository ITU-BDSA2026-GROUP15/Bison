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
        using var repository = new TestRepository(taxon, author,o );
        var service = new ObservationService(repository);

        // act
        // try to create a proposal based on the observation above
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.Id, taxon.TaxonId);

        // assert
        // expect the service to return true and send a proposal
        // also expect the repo to call Save()

        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(proposal,"Proposal was created successfully");
        var p = Assert.IsType<Proposal>(repository.InsertedPost);
        Assert.Same(taxon, p.Taxon);
        Assert.Same(author, p.Author);
        Assert.Same(o, p.Observation);
        Assert.Equal("PROPOSAL TEST", p.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalFromUnknownTaxon()
    {
         // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};
        
        // create an observation we can test proposals on
        // 999 is an invalid taxon-id
    var o = new Observation
        {
            Id = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en fugl",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // taxonID = 999 is invalid 
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.Id, 999);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);

    }

    [Fact]
    public void TryCreateProposalFromUnknownObservation()
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
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // observation with id = 2 does not exist in the testing service 
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", 2, taxon.TaxonId);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalWithEmptyAuthorName()
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
        // leaving author empty
        var proposal = service.TryCreateProposal(" ", author.Email, "PROPOSAL TEST", o.Id, taxon.TaxonId);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalWithWhitespaceText()
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
        // leaving text empty
        var proposal = service.TryCreateProposal(author.Name, author.Email, " ", o.Id, taxon.TaxonId);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalWithInvalidObservationId()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};
        
        // create an observation we can test proposals on
        var o = new Observation
        {
            Id = -1, 
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author);
        var service = new ObservationService(repository);

        // act
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.Id, taxon.TaxonId);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalWithInvalidTaxonId()
    {
       // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = -1, VernacularName = "Solsort"};
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
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.Id, taxon.TaxonId);

        // assert
        Assert.False(proposal,"Proposal was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalReusesExistingAuthor()
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
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST", o.Id, taxon);

        // assert
        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(proposal,"Proposal was created successfully");
        var p = Assert.IsType<Proposal>(repository.InsertedPost);
        Assert.Same(author, p.Author);
        Assert.Same(o, p.Observation);
        Assert.Equal("PROPOSAL TEST", p.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalCreatesNewAuthorWhenUnknown()
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
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        var proposal = service.TryCreateProposal("Line", "Line@test.dk", "PROPOSAL TEST", o.Id, taxon.TaxonId);

        // assert
        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(proposal,"Proposal was created successfully");
        var p = Assert.IsType<Proposal>(repository.InsertedPost);
        Assert.NotSame(author, p.Author);
        Assert.Equal("Line", p.Author.Name);
        Assert.Equal("Line@test.dk", p.Author.Email);
        Assert.Equal("PROPOSAL TEST", p.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateProposalTrimsText()
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
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // added white space to text
        var proposal = service.TryCreateProposal(author.Name, author.Email, "PROPOSAL TEST       ", o.Id, taxon.TaxonId);

        // assert
        Assert.True(proposal,"Proposal was created successfully");
        var p = Assert.IsType<Proposal>(repository.InsertedPost);
        Assert.Equal("PROPOSAL TEST", p.Text);
        Assert.True(repository.SaveCalled);
    }
}