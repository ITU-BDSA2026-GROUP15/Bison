using System;
using System.IO;

class Program {
    public static void Main() {
        
        try {
            using (StreamReader sr = new StreamReader("bison_observe_cli_db.csv")) {
                string line;

                while ((line =sr.ReadLine()) != null) {
                    string[] values = line.Split(',');
                    
                    string author = values[0];
                    string observation = values[1];
                    string timestamp = values[2]; 
                    
                    Console.WriteLine("Author: " + author);
                    Console.WriteLine("Observation: " + observation);
                    Console.WriteLine("Timestamp: " + timestamp);
                }
            }
        }

        catch (Exception e) {
            Console.WriteLine("File could not be read: ");
            Console.WriteLine(e.Message);
        }
    }
}