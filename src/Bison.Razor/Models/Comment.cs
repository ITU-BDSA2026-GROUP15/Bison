namespace Bison.Razor.Models;


//sådan skriver man sub og super klasser!
// sub : super!

// Represents a comment that inherits common properties from Post.
public class Comment : Post {
    // Identifies the observation this comment belongs to.
    public int ObservationId { get; set; }

    // Reference to the observation this comment belongs to.
    public Observation Observation { get; set; } = null!;

}