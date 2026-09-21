using System.Globalization;
using System.Reflection;
using CsvHelper;

namespace Bison.Taxonomy;

// i denne klasse læser vi fra csv filen vi fik på learnit, men den er embedded så den skal lige læses ekstra grundigt så at sige
public class Taxonomy
{
    public Taxonomy()
    {
        _taxons = ReadTaxonsFromResource();
    }

    public static List<Taxon> ReadTaxonsFromResource() // returnerer en liste med en taxon record for hver række i joined.csv
    {
        var assembly = Assembly.GetExecutingAssembly(); // dette skal bruges fordi det er en embedded ressource (i bytes i stedet for bogstaver). det skal vi bruge for at vi ikke kommer til at ændre i filen ved et uheld
        using var stream = assembly.GetManifestResourceStream("Bison.Taxonomy.joined.csv")
                           ?? throw new InvalidOperationException("Resource not found");  // findes den ikke (forkert navn), får vi null, og så kaster vi en fejl

        using var reader = new StreamReader(stream) // her begynder vi faktisk at læse filen
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var taxons = new List<Taxon>(); // opretter tom liste vi putter alle de forskellige arter osv ind i bagefter

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            // ! betyder at vi ved, at feltet aldrig er null. hvis værdierne kan være null, så kører vi den der betingelse ? hvis sand : hvis falsk

            var taxonId = csv.GetField("dwc:taxonID")!;
            var parentId = string.IsNullOrEmpty(csv.GetField("dwc:parentNameUsageID")) ? null : csv.GetField("dwc:parentNameUsageID");
            var rank = csv.GetField("dwc:taxonRank")!;                         // det niveau den er fra order til family, genus, species eller subspecies
            var scientificName = csv.GetField("dwc:scientificName")!;                // det latinske navn
            var vernacularName = string.IsNullOrEmpty(csv.GetField("dwc:vernacularName")) ? null : csv.GetField("dwc:vernacularName"); // det danske navn

            // laver en taxon af de fem felter og lægger den i listen
            taxons.Add(new Taxon(taxonId, parentId, rank, scientificName, vernacularName));
        }

        return taxons;
    }

    public Taxon? GetById(string taxonId)
    {
        return _taxons.FirstOrDefault(t => t.TaxonId == taxonId);
    }

    public Taxon? GetByVernacularName(string name)
    {
        // TODO
    }

    public Taxon? GetSupertaxon(Taxon taxon)
    {
        // TODO
    }

    public List<Taxon> GetSubtaxa(Taxon taxon)
    {
        // TODO
    }

}
