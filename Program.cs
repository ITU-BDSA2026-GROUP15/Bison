using System;
using System.IO;

class Program {
    
    static void Main(string[] args) { //args is what you write in the terminal after the program name, for example: dotnet run observe
        Console.WriteLine(string.Join(", ", args));// https://learn.microsoft.com/en-us/dotnet/api/system.string.join?view=net-10.0
        if(args.Length==0){
            Console.WriteLine("Please provide an argument: 'observe' or 'read'");
            return;
        }
        if(args[0]=="observe"){
            observe(args[1]);// this is a range operator, it takes all the elements from index 1 to the end of the array.
        }
        else{
            read();  
        }

        // used the linkes from the project work week 1
    }
    static void read() {
        string filePath = "bison_observe_cli_db.csv";
        int lineNumber = 0;   

         foreach (string Line in File.ReadLines(filePath)) {
            lineNumber++;
            if(lineNumber==1){
                continue;
            }
            //fails if the observation has a comma in it, because it will split the observation into two parts.
            string[] info = Line.Replace('"',' ').Split(',');// why does it not like ''(char) but like ' ' ??

            string author =info[0];
            string observation = info[1];
            long timestamp = long.Parse(info[2]);//parse the string to long, because the timestamp is a long number.

            //convert the timestamp to a DateTime object, and then convert it to local time.
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;

            // goes from datetime to string, and formats it to the format we want.
            string formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            //printed the author, formatted date and time, and the observation. like we want it. 
            Console.WriteLine(author + " @ " + formattedDateTime + " : " + observation);
        }
    }
    static void observe(string observation){

        //Environment.UserName gets the username of the person running the program, and uses it as the author of the observation
        //from https://learn.microsoft.com/en-us/dotnet/api/system.environment.username?view=net-10.0
        string Author = Environment.UserName; 

        // gets the current time in seconds since 1970-01-01 00:00:00 UTC, and uses it as the timestamp of the observation, link from week 1
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); 

        string newObservation = Author + ",\"" + observation + "\"," + timeStamp + Environment.NewLine;

        Console.WriteLine("Added new observation: " + newObservation);


        //https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp
        //AppenAllText adds the text to the end of the file, and creates the file if it does not exist.
        File.AppendAllText("bison_observe_cli_db.csv", newObservation);
        
        }
    
}