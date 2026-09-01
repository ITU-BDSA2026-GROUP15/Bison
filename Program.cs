using System;
using System.IO;

class Program {
    public static void Main(string[] args) {
        
        if (args[0].Equals("read")) {
            read();
        }

        if (args[0].Equals("observe")) {
            string observation = args[1];
            observe(observation);
        }

    
    }

    private static void read() {
        //StreamReader needs a try/catch block
        
        try {
        
        using (StreamReader sr = new StreamReader("bison_observe_cli_db.csv")) {
                //read headline and not print it in console
                string headLine = sr.ReadLine();
                
                //get the next line
                string line;

                while ((line =sr.ReadLine()) != null) {
                    string[] values = line.Split(',');
                    
                    //format wants author in upper
                    string author = values[0].ToUpper();

                    //removing the quotes to get correct format
                    string observation = values[1].Trim('"');

                    string timestamp = values[2]; 
                    DateTime time = convertTime(timestamp);
                    
                    Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);

                }
            }
        }

        catch (Exception e) {
            Console.WriteLine("File could not be read: ");
            Console.WriteLine(e.Message);
        }

    }

    //method to convert time into correct format
    private static DateTime convertTime(string timestamp) {
        //DTO needs a long, so we need to parse the string into a long
        long unixSeconds = long.Parse(timestamp);
        
        //now using the DTO library to convert unix seconds into actual time
        DateTime time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
        
        //returning the time back to the formatting
        return time;
    }

    public static void observe(string line) {
            string observation = line;
            string author = Environment.UserName;
            
            DateTimeOffset localtime = DateTimeOffset.Now;

            Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);

    }

}