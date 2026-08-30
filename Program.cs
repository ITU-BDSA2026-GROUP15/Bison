using System;
using System.IO;

class Program {
    
    static void Main(string[] args) {
        string filePath = "bison_observe_cli_db.csv";
        int lineNumber = 0;    

        foreach (string Line in File.ReadLines(filePath)) {
            lineNumber++;
            if(lineNumber==1){
                continue;
            }
            string[] info = Line.Replace('"',' ').Split(',');

            string author =info[0];
            string observation = info[1];
            long timestamp = long.Parse(info[2]);

            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;

            string formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine(author + " @ " + formattedDateTime + " : " + observation);
        }
    }
    
}