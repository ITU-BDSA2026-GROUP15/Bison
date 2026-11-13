using SimpleDB;
using Bison.Taxonomy;
//string propFile = "joined.csv";

// indlæs joined.csv, identificer alle taxonIDer tilføj dem til en liste,
// tjek listen igennem når en ny taxon registres i 'propsal'
//et andet sted?


var taxonomy = new Taxonomy();


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build(); //building the webapplication itself (metadata)

string dataDir = app.Configuration["DataDir"] ?? "../..";
string obsFile = $"{dataDir}/bison_observe_cli_db.csv";
string comFile = $"{dataDir}/bison_comment_cli_db.csv";
string propFile = $"{dataDir}/bison_propose_cli_db.csv";

var cheepDb= CSVDatabase<Cheep>.Instance;
var proposalDb = CSVDatabase<Prop>.Instance;

//landing page - redirecting til observations
app.MapGet("/", () => Results.Redirect("/observations"));

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => cheepDb.Read(obsFile));
app.MapPost("/observation", (Cheep observation) => {
    // Servicen tildeler selv ID'et, i stedet for at bruge det
    // klienten sender. Det er lidt unødvendigt at serveren sender et ID,
    // men fordi Cheep bruger ID som parameter, skal der eksistere et ID fra klienten.
    // Det ID, klienten sendte med, bliver ignoreret.
    var existing = cheepDb.Read(obsFile);
    int nextId = existing.Any() ? existing.Max(c => c.ID) + 1 : 0; // if eksisterer noget id, så existing.Max mapping noget, ellers 0
    var stored = observation with { ID = nextId };

    cheepDb.Store(obsFile, stored);
    return stored; });

    app.MapGet("/comments", (int id)=> cheepDb.Read(comFile).Where(comment => comment.ID == id));

    app.MapPost("/comment", (Cheep comment)=>  {

    if (!ProposalValidator.ObservationExists(cheepDb.Read(obsFile), comment.ID)) {
        return Results.BadRequest("Invalid observation ID");
    }
        cheepDb.Store(comFile, comment);
        return Results.Ok(new
        {
            comment,
            file = comFile,
            exists = File.Exists(comFile)
        }
        //return Results.Ok(comment);}
        );
    });



// Proposals
app.MapGet("/proposals", (int id) => proposalDb.Read(propFile).Where(proposal => proposal.ID == id));



app.MapPost("/proposal", (Prop proposal) => {
    // A proposal must refer to an observation that already exists.
    var observations = cheepDb.Read(obsFile);

    if (!ProposalValidator.ObservationExists(observations, proposal.ID)) {
        return Results.BadRequest("Invalid observation ID");
    }

    // The taxon ID must exist in the taxonomy loaded at startup.
    if (!ProposalValidator.TaxonExists(taxonomy, proposal.TaxonID)) {
        return Results.BadRequest("Invalid observation ID");
    }

    // Store the proposal only after both IDs have been validated.
    proposalDb.Store(propFile, proposal);
    return Results.Ok(proposal);
});

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
