using Bison.Core.Models;
using Bison.Infrastructure.Services;

namespace Bison.Razor.Tests;

public class CommentsUnitTests
{
    [Fact]
    public void TryCreateCommentFromValidInput()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // try to create a comment based on the observation above
        var comment = service.TryCreateComment(author.Name, author.Email, "COMMENT TEST", o.PostId);

        // assert
        // expect the service to return true and send a comment
        // also expect the repo to call Save()

        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(comment,"Comment was created successfully");
        var c = Assert.IsType<Comment>(repository.InsertedPost);
        Assert.Same(author, c.Author);
        Assert.Same(o, c.Observation);
        Assert.Equal("COMMENT TEST", c.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentFromUnknownObservation()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
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
        var comment = service.TryCreateComment(author.Name, author.Email, "COMMENT TEST", 2);

        // assert
        Assert.False(comment,"Comment was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentWithEmptyAuthorName()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // leaving author empty
        var comment = service.TryCreateComment(" ", author.Email, "COMMENT TEST", o.PostId);

        // assert
        Assert.False(comment,"Comment was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentWithWhitespaceText()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // leaving text empty
        var comment = service.TryCreateComment(author.Name, author.Email, " ", o.PostId);

        // assert
        Assert.False(comment,"Comment was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentWithInvalidObservationId()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = -1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // observationId <= 0 is rejected before any lookup
        var comment = service.TryCreateComment(author.Name, author.Email, "COMMENT TEST", o.PostId);

        // assert
        Assert.False(comment,"Comment was not created successfully");
        Assert.Null(repository.InsertedPost);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentReusesExistingAuthor()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // "Anne" is already known by the repository
        var comment = service.TryCreateComment(author.Name, author.Email, "COMMENT TEST", o.PostId);

        // assert
        //same betyder, at det er den samme instans af objektet, som vi forventer.
        Assert.True(comment,"Comment was created successfully");
        var c = Assert.IsType<Comment>(repository.InsertedPost);
        Assert.Same(author, c.Author);
        Assert.Same(o, c.Observation);
        Assert.Equal("COMMENT TEST", c.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentCreatesNewAuthorWhenUnknown()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
            Author = author,
            Taxon = taxon,
            Text = "Jeg så en solsort i haven",
            TimeStamp = DateTime.UtcNow
        };

        // creating test repo
        using var repository = new TestRepository(taxon, author, o);
        var service = new ObservationService(repository);

        // act
        // "Line" is not known by the repository
        var comment = service.TryCreateComment("Line", "Line@test.dk", "COMMENT TEST", o.PostId);

        // assert
        Assert.True(comment,"Comment was created successfully");
        var c = Assert.IsType<Comment>(repository.InsertedPost);
        Assert.NotSame(author, c.Author);
        Assert.Equal("Line", c.Author.Name);
        Assert.Equal("Line@test.dk", c.Author.Email);
        Assert.Equal("COMMENT TEST", c.Text);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public void TryCreateCommentTrimsText()
    {
        // arrange
        // repository already knows taxons and authors
        // hence creting an observation with an existing taxon and author
        var taxon = new Taxon{TaxonId = 1, VernacularName = "Solsort"};
        var author = new Author{Name = "Anne", Email = "anne@example.com"};

        // create an observation we can test comments on
        var o = new Observation
        {
            PostId = 1,
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
        var comment = service.TryCreateComment(author.Name, author.Email, "COMMENT TEST       ", o.PostId);

        // assert
        Assert.True(comment,"Comment was created successfully");
        var c = Assert.IsType<Comment>(repository.InsertedPost);
        Assert.Equal("COMMENT TEST", c.Text);
        Assert.True(repository.SaveCalled);
    }
}