namespace Bison.Razor.ModelsDTO;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } =  string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}