using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/observations", () => new Observation("Peter", "I saw the heron again!", 1684229348));
app.Run();
public record Observation(string Author, string Message, long Timestamp);
