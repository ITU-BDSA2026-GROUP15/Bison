using System;
using System.Collections.Generic;
using CommandLine;

//marks the class as the definition of the read command.
//The helpText describes what the command does.
[Verb("read", HelpText ="Read and print the observations from the database.")]

public class ReadOptions{
    
    //collects any unexpected arguments that are provided after the "read" command.
    //required = false allows the correct command to be used without arguments.
    //When the collection contains a value, the parser will throw an error, and the program will print an error message.
     [Value(0, MetaName = "unexpected arguments", Required = false)]
    
    //stores unexpected arguments as a string and initializes it to an empty collection.
    //uses generics to allow the collection to store any number of strings, and not just a single string. thereby using System.Collections.Generic imported.
    public IEnumerable<string> UnexpectedArguments { get; set; }
        = Array.Empty<string>();//this uses system, thereby using System imported;
}
