
//Module: implementing it as a module allows us to import by name in the other classes
module {:extern "Bison.Razor.Models"} Models {
    //:extern means -> do NOT generate new code, and acts as a standin for the c# class
    class {:extern "DafnyTaxon"} Taxon { //Taxon is the name of the class referencing the c# class
        //function tell us whats in the c# class more or less
        //axiom -> dafny has no body to this method and "trusts" the c# class
        function {:extern} {:axiom} isSubTaxon(ancestor: Taxon): bool
    }

    class {:extern "DafnyObservation"} Observation {
        function {:extern} {:axiom} getTaxon(): Taxon
    }
}