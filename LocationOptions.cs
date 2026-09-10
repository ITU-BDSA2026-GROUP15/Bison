using CommandLine;

//defindes location as the command so it can be used in the terminal
[Verb("location",HelpText ="Show observation from the location")]

public class LocationOptions
{
    [Value(0,MetaName ="location",Required =true, HelpText ="The location to search for")]
    public string Location{get;set;} = string.Empty;
}