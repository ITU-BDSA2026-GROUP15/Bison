using CommandLine;

public class ReadOptions{
        [Option('r', "read", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }