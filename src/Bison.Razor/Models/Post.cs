namespace Bison.Razor.Models;

// Fælles egenskaber for observations, comments & proposals.

// abstract so we can not make just a "post" but an Observation, Comment or Proposal
public abstract class Post {

    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }

    // Identifies the author who wrote this post.
    public int AuthorId { get; set; }
    // Reference to the author who wrote this post.
    public Author Author { get; set; } = null!;
}