namespace Bison.Razor.DTOs;

public class ObservationDto
{
    public int Id { get; set; }
    
    public string Text { get; set; } = String.Empty;
    
    public string TimeStamp { get; set; } = String.Empty;
    
    public string AuthorName { get; set; } = string.Empty;
    
    public string TaxonName { get; set; } = string.Empty;
    
}
