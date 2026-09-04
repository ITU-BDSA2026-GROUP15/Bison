using System;

static class UserInterface
{
    public static void PrintObservations(string author, string observation, DateTimeOffset time)
    {
        Console.WriteLine(author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
    }

    public static void PrintObservationAdded(string author, string observation, DateTimeOffset time)
    {
        Console.WriteLine("The following observation has been added to the file:");
        Console.WriteLine(author.ToUpper() + " @ " + time.ToString("MM/dd/yy HH:mm:ss") + ": " + observation);
    }
}