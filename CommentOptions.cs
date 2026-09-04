
//import the CommandLineParser library, which is used to parse command line arguments
  using CommandLine;

//marks the class as the definition of the observe command,
//and provides a help text for the command
[Verb("comment", HelpText = "Add a new comment.")]
  
public class CommentOptions{
    [Value(0, MetaName = "id", Required = true, HelpText = "The ID of the observation to comment on.")]
    public int Id { get; set; }

     //defines the first value after the command "observe".
     //Position 0 means that it is the first positional value. 
     //Required = true means that the user must provide this value, otherwise the parser will throw an error if there is no value.
     //MetaName is the name of the value that will be displayed in the help text/output
    [Value(1, MetaName = "comment", Required = true, HelpText = "The comment to add.")]
        
    //stores the parsed observation, which is a string
    //get and set are used to get and set the value of the property
    //set allows the CommandLineParser to store the text supplied by the user.
    //get allows the program access to retrive that stored text afterwareds.
    //string.Empty gives the property a non-null initial value.
    public string Comment { get; set; } = string.Empty;
}