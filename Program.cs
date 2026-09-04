using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using static UserInterface;
using SimpleDB;
using CommandLine;

class Program {
    public static void Main(string[] args) {//args is what you write in the terminal after the program name, for example: dotnet run observe
        
        parseArguments(args);

    }
    
    static void read() {
        var db = new CSVDatabase <Cheep>("bison_observe_cli_db.csv");
        var cheeps = db.Read();
        
        UserInterface.PrintObservations(cheeps);
        
    }

    
    public static void observe(string observation) {
        var db = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, observation, timestamp);

        db.Store(cheep);
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

            .WithParsed<ObserveOptions>(options =>
            {
                observe(options.Observation);
                
            });
    }

}
