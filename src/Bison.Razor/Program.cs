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
    int nextId = existing.Any() ? existing.Max(c => c.ID) + 1 : 0;
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
        else {
            Console.WriteLine("invalid observation ID");
            //how do i make it invalid????
        }
        
    }
    return matchingProposal;

});


app.MapPost("/proposal", (Prop proposal) => {
    
    var observationExists = false;

    var taxon = taxonomy.GetByID(proposal.TaxonID);
    foreach (var obseration in observationsDb.Read(obsFile)) {
        if(observation.ID == proposal.ID) {
            observationExists = true;
            break;
        }
        
    }

    if(!observationExists) {
        //stop
        //the Results.BadRequest comes from the ASP.NET. 
        //It creats a http-answer med a tatuscode - 400 bad request
        return Results.BadRequest("Invalid observation ID");
        }

    if(taxon==null) {
        return Results.BadRequest("Invalid taxon ID");
    }
    //okay is also something from the ASP.NET
    proposalDb.Store(propFile, proposal);
    return Results.Ok(proposal);
}
);

app.Run();
