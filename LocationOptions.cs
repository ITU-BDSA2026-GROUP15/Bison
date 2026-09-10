using CommandLine;

[Verb("location", HelpText ="Show observation from the location")]

public class LocationOptions
{
    [Value(0,MetaName ="location",Required =true, HelpText ="The location to search for")]
}