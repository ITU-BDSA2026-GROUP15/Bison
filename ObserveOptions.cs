//import the CommandLineParser library, which is used to parse command line arguments
  using CommandLine;

//marks the class as the definition of the observe command,
//and provides a help text for the command
[Verb("observe", HelpText = "Add a new observation.")]
  
public class ObserveOptions {
     //defines the first value after the command "observe".
     //Position 0 means that it is the first positional value. 
     //Required = true means that the user must provide this value, otherwise the parser will throw an error if there is no value.
     //MetaName is the name of the value that will be displayed in the help text/output
    [Value(0, MetaName = "observation", Required = true, HelpText = "The observation to add.")]
        
    //stores the parsed observation, which is a string
    //get and set are used to get and set the value of the property
    //set allows the CommandLineParser to store the text supplied by the user.
    //get allows the program access to retrive that stored text afterwareds.
    //string.Empty gives the property a non-null initial value.
    public string Observation { get; set; } = string.Empty;
}