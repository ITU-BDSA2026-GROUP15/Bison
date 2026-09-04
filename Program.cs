using CsvHelper;
using System;
using System.IO;
using System.Globalization;

using static UserInterface;
using SimpleDB;

class Program {
    public static void Main(string[] args) {
        
        if (args[0].Equals("read")) {
            read(args);
        }

        if (args[0].Equals("observe")) {
            string observation = args[1];
            observe(observation);
        }

    }
    
    static void read(string[] args) {
        var db = new CsvDataBase<Cheep>("bison_observe_cli_db.csv");
        
        
    }


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