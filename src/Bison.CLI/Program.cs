using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

using static UserInterface;
using SimpleDB;
using CommandLine;
using CsvHelper.Configuration.Attributes;
using System.Data.Common;

class Program {

    //HttpClient snakker med Bison.CSVDBService (Bison.Razor) i stedet for at kalde SimpleDB direkte.
    private static readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5273") };

    private static int idTracker; //NEW: ID parsing added to reading observations

    internal static int GetIdTracker() => idTracker;

    public static void Main(string[] args) { //args is what you write in the terminal after the program name, for example: dotnet run observe

        idTracker = InitializeIdTracker();

        parseArguments(args);

    }

       private static int InitializeIdTracker() {

        string file = "../../bison_observe_cli_db.csv";
        var db = CSVDatabase<Cheep>.Instance;

        var cheeps = db.Read(file);

        return cheeps.Any() ? cheeps.Max(c => c.ID) + 1 : 0;
    }

    public static void parseArguments(string[] args){

    //here we give the complete args array to commandlineparser
    //the only two types the parser can produce are either "ReadOptions" or "ObserveOptions"
    Parser.Default.ParseArguments<ReadOptions, ObserveOptions, CommentOptions, DiscussionOptions,LocationOptions>(args)

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
            observe(options.Observation, options.Location);

        })
        .WithParsed<LocationOptions>(options =>
        {
            readLocation(options.Location);
        });

}



    // read() henter nu observationerne fra web servicen (GET /observations) i stedet for at læse CSV-filen direkte.
    private static void read() {
        var cheeps = httpClient.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult();

        UserInterface.PrintObservations(cheeps ?? new List<Cheep>());

    }

    //added location

    internal static void observe(string observation, string location) {
        string file = "../../bison_observe_cli_db.csv";
        var db = CSVDatabase<Cheep>.Instance;

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, idTracker, observation, timestamp, location); //NEW added ID

        db.Store(file, cheep);

        UserInterface.PrintObservationAdded(cheep);

        idTracker++; //Increment ID by 1 for each cheep
    }


    //NEW: function for comment added to program
    internal static void comment(int id, string comment) {
        string file = "../../bison_observe_cli_db.csv";
        var db = CSVDatabase<Cheep>.Instance; //path to CSV file for comments

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, id, comment, timestamp, string.Empty); //Cheep as a comment

        //use the id counter to check if an observation exist
        if (id > idTracker){
            //if ID provided are larger than the max, no observation will exist
            Console.WriteLine("No observations with ID: (" + id + ") currently exists");
            return;
        }

        db.Store(file, cheep);

        UserInterface.PrintCommentAdded(cheep);
    }


    private static void discussion(int obsId){
        string file = "../../bison_comment_cli_db.csv"; // bug? læser ikke fra samme fil som comment og observe.
        var db = CSVDatabase<Cheep>.Instance;

        var cheeps = db.Read(file);

        foreach (Cheep cheep in cheeps)
        {
            //comments are only relevant if they match the id
            if (cheep.ID == obsId){
            UserInterface.PrintObservations(cheeps);
            }
        }
    }
    //db creates acces to the observation database
    //cheeps reads all observations and keeps only those from the requested location
    // the comparison ignores differences between uppercase and lowercase
    //userintercase - displays the matching observation in the terminal
    //NEW: readLocation() henter alle observationer fra web servicen (GET /observations) og filtrerer på
    //location client-side.
    private static void readLocation(string location) {
        var cheeps = httpClient.GetFromJsonAsync<List<Cheep>>("/observations").GetAwaiter().GetResult();
        var matches = (cheeps ?? new List<Cheep>()).Where(cheep => string.Equals(cheep.Location, location, StringComparison.OrdinalIgnoreCase));

        UserInterface.PrintObservations(matches);

    }

}
