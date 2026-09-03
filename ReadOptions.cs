using CommandLine;

[Verb("read", HelpText = "Read the observations from the database.")]

public class ReadOptions{

    [Value(0,MetaName="Readings from the database",Required=false, HelpText="")]

    

    }