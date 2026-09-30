namespace Bison.Razor.Models;
//ovenstående bruges til at fortælle hvor denne klasse hører til, at det er taxon fra denne klasse!
public class Author {
    
    public int ID{ get; set;}
    public string name{ get;  set; } = string.Empty;
    public string email{ get;  set; } = string.Empty;

    //the collection of posts!
    public ICollection<Post> posts{ get; set;} =string.Empty;

}