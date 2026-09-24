using System.ComponentModel.Design;
using Microsoft.VisualBasic;
using SimpleDB;
using Bison.Taxonomy;
using System.Xml.XPath;

//string propFile = "joined.csv";

// indlæs joined.csv, identificer alle taxonIDer tilføj dem til en liste,
// tjek listen igennem når en ny taxon registres i 'propsal'
//et andet sted?


var taxonomy = new Taxonomy();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(); // tilføjede razor pages til programmet

var app = builder.Build(); //building the webapplication itself (metadata)

string obsFile = "../../bison_observe_cli_db.csv";
string comFile = "../../bison_comment_cli_db.csv";
string propFile = "../../bison_propose_cli_db.csv";

var observationDb= CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;
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
app.MapGet("/proposals", (int id) => {
    var allProposals = proposalDb.Read(propFile);
    var matchingProposal = new List<Prop>();
    foreach(var proposal in allProposals) {
        if (proposal.ID == id) {
            matchingProposal.Add(proposal);
        }

    }
    return matchingProposal;

});


app.MapPost("/proposal", (Prop proposal) => {
    // A proposal must refer to an observation that already exists.
    var observations = observationDb.Read(obsFile);

    if (!ProposalValidator.ObservationExists(observations, proposal.ID)) {
        return Results.BadRequest("Invalid observation ID");
    }

    // The taxon ID must exist in the taxonomy loaded at startup.
    if (!ProposalValidator.TaxonExists(taxonomy, proposal.TaxonID)) {
        return Results.BadRequest("Invalid taxon ID");
    }

    // Store the proposal only after both IDs have been validated.
    proposalDb.Store(propFile, proposal);
    return Results.Ok(proposal);
});

app.Run();

// Gør den (ellers implicit genererede) Program-klasse offentlig og tilgængelig, så
// WebApplicationFactory<Program> kan bruges fra Bison.Razor.Tests til at køre servicen in-memory.
public partial class Program { }
