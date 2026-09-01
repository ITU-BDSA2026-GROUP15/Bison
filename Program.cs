using System;
using System.IO;

class Program {
    public static void Main() {
    
        //StreamReader needs a try/catch block
        try {
            readFile();
        }

        catch (Exception e) {
            Console.WriteLine("File could not be read: ");
            Console.WriteLine(e.Message);
        }
    }

    public static void readFile() {
        using (StreamReader sr = new StreamReader("bison_observe_cli_db.csv")) {
                //read headline and not print it in console
                string headLine = sr.ReadLine();
                
                //get the next line
                string line;

                while ((line =sr.ReadLine()) != null) {
                    string[] values = line.Split(',');
                    
                    //format wants author in upper
                    string author = values[0].ToUpper();
                    string observation = values[1];
                    string timestamp = values[2]; 
                    
                    Console.WriteLine(author + " @ " + timestamp + " " + observation);

                }
            }
    }

/*
    public static append (string line) {
        //creating a path to the file where we want to append
        using (StreamWriter sw = File.CreateText(sr)) {
            sw.WriteLine(author);
            sw.Write(" @ ") ;
            sw.Write(timestamp);
            sw.Write(observation);
        }
    }
*/
}