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
        var db = new CSVDatabase <Cheep>("bison_observe_cli_db.csv");
        var cheeps = db.Read();
        
        UserInterface.PrintObservations(cheeps);
    }


    public static void observe(string observation) {
         var db = new CSVDatabase <Cheep>("bison_observe_cli_db.csv");
         var cheeps = db.Store(observation);

         UserInterface.PrintObservationAdded(cheeps.Observation);
        
    }
}