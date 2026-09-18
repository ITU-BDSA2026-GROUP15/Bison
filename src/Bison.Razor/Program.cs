using Microsoft.AspNetCore.Builder;
using SimpleDB.CSVDatabase;


var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

string obsFile = "bison_observe_cli_db.csv";
string comFile = "bison_comment_cli_db.csv";

var observationDb= new CSVDatabase<Cheep>.Instance(CSVDatabase.cs);
var commentDb = new CSVDatabase<Cheep>.Instance(CSVDatabase.cs);

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile)); 
app.MapPost("/observation", (Cheep observation) => observationDb.Store(observation));

app.MapGet("/comments", (int id)=> commentDb.Read(comFile));
app.MapPost("/comment", (Cheep comment)=>  commentDb.Store(comment));
//er der noget som siger return all comment?
//hvordan ved vi at den kommer ind i appsettings.json?

app.Run();
public record Observation(string Author, string Message, long Timestamp);
