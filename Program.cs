using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using System.Linq;

using static UserInterface;
using SimpleDB;
using CommandLine;
using CsvHelper.Configuration.Attributes;

class Program {

    private static int id = 0;

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

            .WithParsed<CommentOption>(options =>
            {
                comment(options.Comment);
            })

            .WithParsed<DiscussionOption>(options =>
            {
                discussion(options.Discussion);
            })

            .WithParsed<ObserveOptions>(options =>
            {
                observe(options.Observation);
                
            });
    }
    
    private static void read() {
        var db = new CSVDatabase <Cheep>("bison_observe_cli_db.csv");
        var cheeps = db.Read();

        //NEW: ID parsing added to reading observations
        int id = int.Parse(values[1]);
        
        UserInterface.PrintObservations(cheeps);
        
    }

    
    private static void observe(string observation) {
        var db = new CSVDatabase<Cheep>("bison_observe_cli_db.csv");

        string author = Environment.UserName;
        DateTimeOffset now = DateTimeOffset.Now;
        long timestamp = now.ToUnixTimeSeconds();

        var cheep = new Cheep(author, observation, timestamp);

        db.Store(cheep);
        
        UserInterface.PrintObservationAdded(cheep);
    }

    //NEW: function for comment added to program
    //Refactor later to fit new structure of code
    private static void Comment(string comment, int comId) {
        //use the id counter to check if an observation exist
        
        if (comId > id){
            //if ID provided are larger than the max, no observation will exist
            Console.WriteLine("No observations with ID: " + comId + "currently exists");
            return;
        }
        string author = Environment.UserName;
        DateTimeOffset localtime = DateTimeOffset.Now;
        long time = localtime.ToUnixTimeSeconds();

        //NEW FILE: bison_comment, CSV where comments are added
        using (StreamWriter sw = File.AppendText("bison_comment_cli_db.csv")) {
                sw.WriteLine($"\"{author}, {comId}, \"{comment}\",{time}\"");
            }

        Console.WriteLine("The following comment has been added to the file:");
        Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + comId + " " + comment);
    }

    //NEW: function for listing comments is now added
    //Refactor later
    private static void Discussion(int comId){
        try {
        
            using (StreamReader sr = new StreamReader("bison_comment_cli_db.csv")) {
                    //read headline and not print it in console
                    string headLine = sr.ReadLine();
                    
                    //get the next line
                    string line;

                    while ((line =sr.ReadLine()) != null) {
                        string[] values = line.Trim('"').Split(',');
                        
                        int obsId = int.Parse(values[1]);

                        //comments are only relevant if they match the id
                        if (obsId == comId){
                            string author = values[0].ToUpper();

                            string comment = values[2];

                            string timestamp = values[3]; 
                            DateTimeOffset time = ConvertTime(timestamp);
                        
                            Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "ID:" + comId + " " + comment);
                        }
                    }
            }
        }

        catch (Exception e) {
            Console.WriteLine("No comments have been made for this observation");
            Console.WriteLine(e.Message);
        }
    }

}
