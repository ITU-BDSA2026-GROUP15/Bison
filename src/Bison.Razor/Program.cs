using Microsoft.VisualBasic;
using SimpleDB;


var builder = WebApplication.CreateBuilder(args);

var app = builder.Build(); //building the webapplication itself (metadata)

string obsFile = "../../bison_observe_cli_db.csv";
string comFile = "../../bison_comment_cli_db.csv";


var observationDb = CSVDatabase<Cheep>.Instance;
var commentDb = CSVDatabase<Cheep>.Instance;

// skal sende et kald til loggede observationer i stedet?? -> connecte dette til simpledb?
app.MapGet("/observations", () => observationDb.Read(obsFile)); 

app.MapPost("/observation", (Cheep observation) => {
    observationDb.Store(obsFile,observation); 

});

//MAPGET -> gets the observations/ the comments
// MAPPOST -> create an observations/ a comment

//app.MapGet("/comments", (int id)=> commentDb.Read(comFile));
//app.MapGet("/comments", () => commentDb.Read(comFile)); 

app.MapGet("/comments", (int id) => {
    var allComments = commentDb.Read(comFile);
    var matchingComment = new List<Cheep>();
    foreach(var comment in allComments)
    {
        if (comment.ID == id)
        {
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
