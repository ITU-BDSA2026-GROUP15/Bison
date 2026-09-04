using System;

static class UserInterface
{
    static void PrintObservations(string author, DateTimeOffset time, string observation)
    {
        Console.WriteLine(author + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
    }

    static void PrintObservationAdded(string author, DateTimeOffset localtime, string observation)
    {
        Console.WriteLine("The following observation has been added to the file:");
        Console.WriteLine(author + " @ " + localtime.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
    }
}