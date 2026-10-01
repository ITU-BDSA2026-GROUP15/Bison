namespace Bison.Razor.ModelsDTO;

public class ObservationDto
{
    public int Id { get; set; }

    public string Text { get; set; } = String.Empty;

    public string TimeStamp { get; set; } = String.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string DanishVernacularName { get; set; } = string.Empty;
}