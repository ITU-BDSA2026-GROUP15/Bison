using System;
using System.Collections.Generic;
using CommandLine;

//marks the class as the definition of the read command.
//The helpText describes what the command does.
[Verb("discussion", HelpText ="Read and print the discussions (comments) from the database.")]

public class DiscussionOptions{
    
    //collects any unexpected arguments that are provided after the "read" command.
    //required = false allows the correct command to be used without arguments.
    //When the collection contains a value, the parser will throw an error, and the program will print an error message.
     [Value(0, MetaName = "observation ID for related comments", Required = false)]
    
    //stores unexpected arguments as a string and initializes it to an empty collection.
    //uses generics to allow the collection to store any number of strings, and not just a single string. thereby using System.Collections.Generic imported.
    public IEnumerable<int> UnexpectedArguments { get; set; }
        = Array.Empty<int>();//this uses system, thereby using System imported;
}