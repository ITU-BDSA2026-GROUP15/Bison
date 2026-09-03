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
                    string[] values = line.Trim('"').Split(',');
                    
                    //format wants author in upper
                    string author = values[0].ToUpper();

                    int id = (int) values[1];

                    //removing the quotes to get correct format
                    string observation = values[2].Trim('"');

                    string timestamp = values[3]; 
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
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime();
        
        //returning the time back to the formatting
        return time;
    }

    public static void observe(string line) {
            // NEW: added counter for ID on observations
            int id = 0;
            
            string observation = line;
            string author = Environment.UserName;
            
            DateTimeOffset localtime = DateTimeOffset.Now;
            long time = localtime.ToUnixTimeSeconds();
           
            using (StreamWriter sw = File.AppendText("bison_observe_cli_db.csv")) {
                //NEW: added ID to the output line
                sw.WriteLine($"\"{author}, {id}, \"\"{observation}\"\",{time}\"");
            }

            Console.WriteLine("The following observation has been added to the file:");
            Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + id + observation);
            
            //NEW: increment ID for every observation registered
            id++;
    }

}