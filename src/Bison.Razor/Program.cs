using System.ComponentModel.Design;
using Microsoft.VisualBasic;
using SimpleDB;

//string propFile = "joined.csv";

// indlæs joined.csv, identificer alle taxonIDer tilføj dem til en liste,
// tjek listen igennem når en ny taxon registres i 'propsal'
//vil jeg gøre et andet sted
//hvad sagde du?


List<string> taxon_IDs = Taxons.start("../../taxons/joined.csv");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build(); //building the webapplication itself (metadata)

string obsFile = "../../bison_observe_cli_db.csv";
string comFile = "../../bison_comment_cli_db.csv";
string propFile = "../../bison_propose_cli_db.csv";

var observationDb = CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;
var proposalDb = CSVDatabase<Prop>.Instance;

//MAPGET -> gets the observations/ the comments
// MAPPOST -> create an observations/ a comment

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile)); 

app.MapPost("/observation", (Cheep observation) => {
    observationDb.Store(obsFile,observation); 
});





//app.MapGet("/comments", (int id)=> commentDb.Read(comFile));
//app.MapGet("/comments", () => commentDb.Read(comFile)); 

app.MapGet("/comments", (int id) => {
    var allComments = commentDb.Read(comFile);
    var matchingComment = new List<Cheep>();
    foreach(var comment in allComments){
        
        if (comment.ID == id){
            matchingComment.Add(comment);
        }
    }
    return matchingComment;
});
    
//post, 
app.MapPost("/comment", (Cheep comment)=>  { 
    commentDb.Store(comFile,comment);
});

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


app.MapPost("/proposal", (Prop TaxonID) => {


proposalDb.Store(propFile, TaxonID);
}
);

app.Run();
