  using commandLine;

//marks the class as the definition of the observe command, and provides a help text for the command
[Verb("observe", HelpText = "Adda new observation.")]
  
public class ObserveOptions{

     //defines the first value after the command "observe".
     //Position 0 means that it is the first positional value 
     //required = true means that the user must provide this value, otherwise the program will not run
     //metaName is the name of the value that will be displayed in the help text/output
    [Value(0, MetaName = "observation", Required = true, HelpText = "The Observation to add.")]
        
    //stores the value of the observation in the property Observation, which is a string
    //get and set are used to get and set the value of the property
    //set allows the commandLinePaser to store the text supplied by the user.
    //get allows the program access to retrive that stored text afterwareds.
    public string Observation { get; set; } = string.Empty;

    }