using System;
using System.IO;

class Program {
    // NEW: added counter for ID on observations

    private static int id = 0;
    public static void Main(string[] args) {
        
        if (args[0].Equals("read")) {
            Read();
        }

        if (args[0].Equals("observe")) {
            string observation = args[1];
            Observe(observation);
        }

        if (args[0].Equals("comment"))
        {
            int commentId = Int32.Parse(args[1]);
            string com = args [2];
            Comment(com, commentId);

        }

        //NEW: printing comments related to an ID
        if (args[0].Equals("discussion"))
        {
            int id = Int32.Parse(args[1]);
            Discussion(id);
        }

    }

    private static void Read() {
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
                    string observation = values[2];

                    string timestamp = values[3]; 
                    DateTimeOffset time = ConvertTime(timestamp);
                    
                    Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "ID:" + id + " " + observation.Trim('"'));

                }
            }
        }

        catch (Exception e) {
            Console.WriteLine("File could not be read: ");
            Console.WriteLine(e.Message);
        }

    }

    //method to convert time into correct format
    private static DateTimeOffset ConvertTime(string timestamp) {
        //DTO needs a long, so we need to parse the string into a long
        long unixSeconds = long.Parse(timestamp);
        
        //now using the DTO library to convert unix seconds into actual time
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime();
        
        //returning the time back to the formatting
        return time;
    }

    private static void Observe(string line) {            
            string observation = line;
            string author = Environment.UserName;
            
            DateTimeOffset localtime = DateTimeOffset.Now;
            long time = localtime.ToUnixTimeSeconds();
           
            using (StreamWriter sw = File.AppendText("bison_observe_cli_db.csv")) {
                //NEW: added ID to the output line
                sw.WriteLine($"\"{author}, {id}, \"\"{observation}\"\",{time}\"");
            }

            Console.WriteLine("The following observation has been added to the file:");
            Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + id + " " + observation);
            
            //NEW: increment ID for every observation registered
            id++;
    }


    //NEW: function for comment added to program
    //Refactor later to fit new structure of code
    private static void Comment(string comment, int comId) {
        //use the id counter to check if an observation exist
        
        if (comId > id){
            //if ID provided are larger than the max, no observation will exist
            Console.WriteLine("No observations with ID: " + comId + "currently exists");
            return;
        }
        string author = Environment.UserName;
        DateTimeOffset localtime = DateTimeOffset.Now;
        long time = localtime.ToUnixTimeSeconds();

        //NEW FILE: bison_comment, CSV where comments are added
        using (StreamWriter sw = File.AppendText("bison_comment_cli_db.csv")) {
                sw.WriteLine($"\"{author}, {comId}, \"{comment}\",{time}\"");
            }

        Console.WriteLine("The following comment has been added to the file:");
        Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + comId + " " + comment);
    }

    //NEW: function for listing comments is now added
    //Refactor later
    private static void Discussion(int comId){
        try {
        
            using (StreamReader sr = new StreamReader("bison_comment_cli_db.csv")) {
                    //read headline and not print it in console
                    string headLine = sr.ReadLine();
                    
                    //get the next line
                    string line;

                    while ((line =sr.ReadLine()) != null) {
                        string[] values = line.Trim('"').Split(',');
                        
                        int obsId = int.Parse(values[1]);

                        //comments are only relevant if they match the id
                        if (obsId == comId){
                            string author = values[0].ToUpper();

                            string comment = values[2];

                            string timestamp = values[3]; 
                            DateTimeOffset time = ConvertTime(timestamp);
                        
                            Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + "ID:" + comId + " " + comment);
                        }
                    }
            }
        }

        catch (Exception e) {
            Console.WriteLine("No comments have been made for this observation");
            Console.WriteLine(e.Message);
        }
    }

}