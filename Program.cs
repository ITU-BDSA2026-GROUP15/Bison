using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using System.Linq;

using static UserInterface;
using SimpleDB;
using CommandLine;
using CsvHelper.Configuration.Attributes;
using System.Data.Common;

class Program {

    private static int idTracker = 0; //NEW: ID parsing added to reading observations

    public static void Main(string[] args) {//args is what you write in the terminal after the program name, for example: dotnet run observe
        
        parseArguments(args);

    }

        public static void parseArguments(string[] args){

        //here we give the complete args array to commandlineparser
        //the only two types the parser can produce are either "ReadOptions" or "ObserveOptions"
        Parser.Default.ParseArguments<ReadOptions, ObserveOptions>(args)

        //the parser only runs when the user writes "read"

            .WithParsed<ReadOptions>(options =>
            {
                //makes sure that the program does not execute if "read" recives extra arguments
                //with a error message, and returns to the terminal without executing the read() method
                if (options.UnexpectedArguments.Any()){
                    Console.WriteLine("Error: Unexpected arguments provided for the 'read' command.");
                    return;
                }

                read();
            })

            .WithParsed<CommentOptions>(options =>
            {
                comment(options.Id, options.Comment);
            })

            .WithParsed<DiscussionOptions>(options =>
            {
                discussion(options.ObservationId);
            })

            .WithParsed<ObserveOptions>(options =>
            {
                observe(options.Observation);
                
            });
    }
    
    private static void read() {
        var db = new CSVDatabase <Cheep>("bison_observe_cli_db.csv");
        var cheeps = db.Read();
        
        UserInterface.PrintObservations(cheeps);
        
    }

    
    private static void observe(string observation) {
        var db = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, idTracker, observation, timestamp); //NEW added ID

        db.Store(cheep);
        
        UserInterface.PrintObservationAdded(cheep);
        
        idTracker++; //Increment ID by 1 for each cheep
    }

    //NEW: function for comment added to program
    private static void comment(int id, string comment) {
        var db = new CSVDatabase<Cheep>("bison_comment_cli_db.csv"); //path to CSV file for comments

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, id, comment, timestamp); //Cheep as a comment

        //use the id counter to check if an observation exist
        if (id > idTracker){
            //if ID provided are larger than the max, no observation will exist
            Console.WriteLine("No observations with ID: (" + id + ")currently exists");
            return;
        }

        db.Store(cheep);

        UserInterface.PrintCommentAdded(cheep);
    }

    //NEW: function for listing comments is now added
    private static void discussion(int obsId){

        var db = new CSVDatabase <Cheep>("bison_comment_cli_db.csv");
        var cheeps = db.Read();
        
        foreach (Cheep cheep in cheeps)
        {
            //comments are only relevant if they match the id
            if (cheep.ID == obsId){
            UserInterface.PrintObservations(cheeps);
            }
        }
    }

}
