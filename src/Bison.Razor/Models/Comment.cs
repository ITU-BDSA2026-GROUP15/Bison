namespace Bison.Razor.Models;


//sådan skriver man sub og super klasser!
// sub : super!
public class Comment : Post {
    public int ObservationId { get; set; }
    public Observation Observation { get; set; } = null!;

}