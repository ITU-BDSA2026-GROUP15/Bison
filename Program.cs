using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using static UserInterface ;

class Program {
    public static void Main(string[] args) {
        
        if (args[0].Equals("read")) {
            Read();
        }

        if (args[0].Equals("observe")) {
            string observation = args[1];
            observe(observation);
        }

    }
/*
    //method to convert time into correct format
    public static DateTimeOffset convertTime(long timestamp) {
        //DTO needs a long, so we need to parse the string into a long
        long unixSeconds = timestamp;
        //Removed long.parse - since it is already long 
        
        //now using the DTO library to convert unix seconds into actual time
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime();
        
        //returning the time back to the formatting
        return time;
    }
    */

    public static void observe(string observation) {
            
            string author = Environment.UserName;
            
            DateTimeOffset now = DateTimeOffset.Now;
            
            long timestamp = now.ToUnixTimeSeconds();
            
            var cheep = new Cheep(author, observation, timestamp);
            using (var writer= new StreamWriter("bison_observe_cli_db.csv", true))
            using (var csv= new CsvWriter(writer, CultureInfo.InvariantCulture)) {
            csv.WriteRecord(cheep);
            csv.NextRecord();

            PrintObservationAdded(cheep.Author, cheep.Observation, now);
        
    }

}
}