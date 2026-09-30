namespace Bison.Razor.Models;
//ovenstående bruges til at fortælle hvor denne klasse hører til, at det er taxon fra denne klasse! (meget vigtig!)

// Represents an author and the posts they have written.
public class Author {
    
    public int ID{ get; set;}
    public string name{ get;  set; } = string.Empty;
    public string email{ get;  set; } = string.Empty;

   
    // Contains the author's observations, comments, and proposals.
    public ICollection<Post> posts{ get; set;} =string.Empty;

}