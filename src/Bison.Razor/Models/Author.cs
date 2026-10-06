namespace Bison.Razor.Models;
//ovenstående bruges til at fortælle hvor denne klasse hører til, at det er taxon fra denne klasse! (meget vigtig!)

// Represents an author and the posts they have written.
public class Author {
    //maybe should be small ID??!
    public int AuthorId{ get; set;}
    public string Name{ get;  set; } = string.Empty;
    public string Email{ get;  set; } = string.Empty;


    // Contains the author's observations, comments, and proposals.
    public ICollection<Post> Posts{ get; set;} = new List<Post>();

}
