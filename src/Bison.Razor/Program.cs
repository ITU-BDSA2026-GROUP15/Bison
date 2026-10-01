using System.ComponentModel.Design;
using Microsoft.VisualBasic;
using System.Xml.XPath;
using System.Linq.Expressions;
using Bison.Core;

//string propFile = "joined.csv";

// indlæs joined.csv, identificer alle taxonIDer tilføj dem til en liste,
// tjek listen igennem når en ny taxon registres i 'propsal'
//et andet sted?

// Opretter taxon, der bruges til at kontrollere taxon-ID'er.
var taxonomy = new Taxonomy();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); // tilføjede razor pages til programmet

var envPath  = Environment.GetEnvironmentVariable("BISONDBPATH");   // kan måske være null
var tempPath = Path.Combine(Path.GetTempPath(), "bison.db");        // fallback
var dbPath   = envPath ?? tempPath;

//NEW CHECK FOR PATH IF IT DOESN'T EXIST YET
    if (!File.Exists(dbPath))
    {
        //makes a connection to the sqlite file on the dbPath
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        //lets us edit and send SQL to the database
        conn.Open();

        //runs through the two sql files, first schema, then dump
        foreach (var file in new[] { "schema.sql", "dump.sql" })
        {
            using var cmd = conn.CreateCommand();
            //reads everything from data-folder and use it as SQL-text
            cmd.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", file));
            //non-quer means we don't expect rows returned, but only updated or inserted
            cmd.ExecuteNonQuery();
        }
    }

builder.Services.AddSingleton<IObservationService, ObservationService>(); // hvis man kalder dette med en interface giver den en instance as observationservice.
builder.Services.AddSingleton(new DBFacade(dbPath)); //vis den kalder den med en dbfacade

var app = builder.Build(); //building the webapplication itself (metadata)

app.MapRazorPages(); // kobler URL til razor pages
app.UseStaticFiles(); // kobler browseren til filerne i wwwroot -> altså css styling osv så det ikke bare er tekst

string obsFile = "../../bison_observe_cli_db.csv";
string comFile = "../../bison_comment_cli_db.csv";
string propFile = "../../bison_propose_cli_db.csv";

var observationDb= CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;
//Her fortæller Prop, hvilken type data proposal-databasen gemmer og læser.
var proposalDb = CSVDatabase<Prop>.Instance;

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile));
app.MapPost("/observation", (Cheep observation) => {
    // Servicen tildeler selv ID'et, i stedet for at bruge det
    // klienten sender. Det er lidt unødvendigt at serveren sender et ID,
    // men fordi Cheep bruger ID som parameter, skal der eksistere et ID fra klienten.
    // Det ID, klienten sendte med, bliver ignoreret.
    var existing = observationDb.Read(obsFile);
    int nextId = existing.Any() ? existing.Max(c => c.ID) + 1 : 0; // if eksisterer noget id, så existing.Max mapping noget, ellers 0
    var stored = observation with { ID = nextId };

    observationDb.Store(obsFile, stored);
    return stored; });

app.MapGet("/comments", (int id)=> commentDb.Read(comFile).Where(comment => comment.ID == id));
//app.MapGet("/comments", () => commentDb.Read(comFile));

app.MapPost("/comment", (Cheep comment)=>  {
    /*return*/ commentDb.Store(comFile, comment);});





// Proposals
// Returnerer forslag, der hører til det angivne observations-ID.
app.MapGet("/proposals", (int id) => {
    var allProposals = proposalDb.Read(propFile);
    var matchingProposal = new List<Prop>();//Her indeholder listen Prop-objekter.
    // Gennemgår alle forslag og tilføjer dem med et matchende ID til listen.
    foreach(var proposal in allProposals) {
        if (proposal.ID == id) {
            matchingProposal.Add(proposal);
        }

    }
    return matchingProposal;

});

//look at it!!!!!! need a different comparesons?
// Modtager JSON, som ASP.NET automatisk omsætter til en Prop-record.
app.MapPost("/proposal", (Prop proposal) => {//Her oversætter ASP.NET automatisk JSON fra HTTP-requesten til en Prop-record.
    // A proposal must refer to an observation that already exists.
    var observations = observationDb.Read(obsFile);
    // Afviser forslaget med HTTP 400, hvis observationen ikke findes.
    if (!ProposalValidator.ObservationExists(observations, proposal.ID)) {
        return Results.BadRequest("Invalid observation ID");
    }

    // The taxon ID must exist in the taxonomy loaded at startup.
    // // Afviser forslaget med HTTP 400, hvis taxon-ID'et ikke findes.
    if (!ProposalValidator.TaxonExists(taxonomy, proposal.TaxonID)) {
        return Results.BadRequest("Invalid taxon ID");
    }
    // Gemmer forslaget, når begge ID'er er gyldige.
    // Store the proposal only after both IDs have been validated.
    proposalDb.Store(propFile, proposal);//gemmer den 
    // Returnerer HTTP 200 sammen med det gemte forslag.
    return Results.Ok(proposal);
});
// Gør Razor-siderne tilgængelige via deres URL'er.

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
