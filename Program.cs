using System;
using System.IO;

class Program 
{
    
    static void Main(string[] args)
    {
        string filePath = "bison_observe_cli_db.csv";
    
        foreach (string Line in File.ReadLines(filePath))
        {
        Console.WriteLine(Line);
        }
    }
    
}