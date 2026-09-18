using System.ComponentModel.Design;
using Microsoft.VisualBasic;
using SimpleDB;


var builder = WebApplication.CreateBuilder(args);

var app = builder.Build(); //building the webapplication itself (metadata)

string obsFile = "../../bison_observe_cli_db.csv";
string comFile = "../../bison_comment_cli_db.csv";
string propFile = "../../bison_propose_cli_db.csv";

var observationDb = CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;
var proposalDb = CSVDatabase<Cheep>.Instance;

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile)); 

app.MapPost("/observation", (Cheep observation) => {
    observationDb.Store(obsFile,observation); 
});

// Proposals
app.MapGet("/proposals", (taxonID) => proposalDb.Read(propFile));

app.MapPost("/proposal", (Cheep observation) => {
    var allProposals = proposalDb.Read(propFile);
    var matchingProposal = new List<Cheep>();
    foreach(var proposal in allProposals) {
        if (proposal.ID == id) {
            matchingProposal.Add(proposal);
        }
        else {
            Console.WriteLine("invalid observation ID");
        }
        return matchingPorposal;
    }

    proposalDb.Store(propFile,proposal); 
});

//MAPGET -> gets the observations/ the comments
// MAPPOST -> create an observations/ a comment

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

app.Run();
