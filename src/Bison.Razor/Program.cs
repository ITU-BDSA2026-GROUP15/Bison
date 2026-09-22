using SimpleDB;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build(); //building the webapplication itself (metadata)

string obsFile = "bison_observe_cli_db.csv";
string comFile = "bison_comment_cli_db.csv";

var observationDb= CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile));
app.MapPost("/observation", (Cheep observation) => {
    // Servicen tildeler selv ID'et, i stedet for at bruge det
    // klienten sender. Det er lidt unødvendigt at serveren sender et ID,
    // men fordi Cheep bruger ID som parameter, skal der eksistere et ID fra klienten.
    // Det ID, klienten sendte med, bliver ignoreret.
    var existing = observationDb.Read(obsFile);
    int nextId = existing.Any() ? existing.Max(c => c.ID) + 1 : 0;
    var stored = observation with { ID = nextId };

    observationDb.Store(obsFile, stored);
    return stored; });

app.MapGet("/comments", (int id)=> commentDb.Read(comFile).Where(comment => comment.ID == id));
//app.MapGet("/comments", () => commentDb.Read(comFile));

app.MapPost("/comment", (Cheep comment)=>  {
    /*return*/ commentDb.Store(comFile, comment);});

//er der noget som siger return all comment?
//hvordan ved vi at den kommer ind i appsettings.json?

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
