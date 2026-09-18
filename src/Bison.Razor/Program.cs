using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var observationDB= new CSVDatabase<Cheep>("bison_observe_cli_db.csv");
var commentDb = new CSVDatabase<Cheep>("bison_comment_cli_db.csv");


// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read("bison_observe_cli_db.csv")); 
app.MapPost("/observation", (Cheep observation) => observationDb.Store(observation));

app.MapGet("/comments", (int id)=> Comment("Author",id, "Observation"));
app.MapPost("/comment", ()=> new Comment("Author",id, "Observation"));
//er der noget som siger return all comment?
//hvordan ved vi at den kommer ind i appsettings.json?

app.Run();
public record Observation(string Author, string Message, long Timestamp);
