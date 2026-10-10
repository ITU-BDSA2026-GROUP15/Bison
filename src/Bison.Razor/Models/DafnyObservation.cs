namespace Bison.Razor.Models;

// C#-implementeringen af {:extern}-klassen fra src/Dafny/Observation.dfy.
public class DafnyObservation
{
    // Den originale observation.
    public Observation Observation { get; }

    public DafnyObservation(Observation observation)
    {
        // Dafny-typer kan aldrig være null, så vi fejler tidligt (i konstruktøren), hvis der er en fejl.
        Observation = observation ?? throw new ArgumentNullException(nameof(observation));
    }

    // Kaldes af den genererede FilterBy-kode. Har ikke endnu
    public DafnyTaxon getTaxon()
    {
        return new DafnyTaxon(Observation.Taxon);
    }
}