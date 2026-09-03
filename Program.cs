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

                    //NEW: ID parsing added to reading observations
                    int id = int.Parse(values[1]);

                    //removing the quotes to get correct format
                    string observation = values[2].Trim('"');

                    string timestamp = values[3]; 
                    DateTimeOffset time = convertTime(timestamp);
                    
                    Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "ID:" + id + " " + observation);

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

    private static void observe(string line) {
            // NEW: added counter for ID on observations
            int id = maxId();
            
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

    //NEW: maxId
    //only works if no observations have been logged initially
    private static int maxId()
    {
        int id = 3;
        int currentId = id;
        id++;

        return currentId;
    }

    //NEW: function for comment added to program
    //Refactor later to fit new structure of code
    private static void comment(string comment, int id) {
        //use the id counter to check if an observation exist
        int max = maxId();
        
        if (id > max){
            //if ID provided are larger than the max, no observation will exist
            Console.WriteLine("No observations with ID: " + id + "currently exists");
            return;
        }
        string author = Environment.UserName;
        DateTimeOffset localtime = DateTimeOffset.Now;
        long time = localtime.ToUnixTimeSeconds();

        //NEW FILE: bison_comment, CSV where comments are added
        using (StreamWriter sw = File.AppendText("bison_comment_cli_db.csv")) {
                sw.WriteLine($"\"{author}, {id}, \"\"{comment}\"\",{time}\"");
            }

        Console.WriteLine("The following comment has been added to the file:");
        Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + id + comment);
    }

}