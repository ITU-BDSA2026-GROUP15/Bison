using System;
using System.IO;

class Program {
    public static void Main(string[] args) {//args is what you write in the terminal after the program name, for example: dotnet run observe
        
        if(args.Length == 0) {//default case if no arguments are provided
            Console.WriteLine("Please provide an argument: read or observe");
            return;
        }
        if (args[0].Equals("read")) {
            read();
        }

        if (args[0].Equals("observe")) {
            string observation = args[1];// this is a range operator, it takes all the elements from index 1 to the end of the array.
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
                    string[] values = line.Trim('"').Split(',');
                    
                    //format wants author in upper
                    string author = values[0].ToUpper();

                    //removing the quotes to get correct format
                    string observation = values[1].Trim('"');

                    string timestamp = values[2]; 


                    DateTimeOffset time = convertTime(timestamp);
                    
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
    private static DateTimeOffset convertTime(string timestamp) {
        //DTO needs a long, so we need to parse the string into a long
        long unixSeconds = long.Parse(timestamp);
        
        //now using the DTO library to convert unix seconds into actual time
        //is for converting the timetamp into local time, so that it is easier to read for the user
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime();
        
        //returning the time back to the formatting
        return time;
    }

    public static void observe(string observation) {
    
            //Environment.UserName gets the username of the person running the program, and uses it as the author of the observation
            //from https://learn.microsoft.com/en-us/dotnet/api/system.environment.username?view=net-10.0
            string author = Environment.UserName;
            
            // gets the current time in seconds since 1970-01-01 00:00:00 UTC, and uses it as the timestamp of the observation, link from week 1
            DateTimeOffset localtime = DateTimeOffset.Now;


            long time = localtime.ToUnixTimeSeconds();
           
            //https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp
           //AppendText is used to append the new observation to the end of the file, and not overwrite the existing observations
            using (StreamWriter sw = File.AppendText("bison_observe_cli_db.csv")) {
                sw.WriteLine($"\"{author},\"\"{observation}\"\",{time}\"");
            }

            Console.WriteLine("The following observation has been added to the file:");
            Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
    }

}