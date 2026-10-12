class {:extern} DafnyTaxon {
    function {:extern} isSubTaxon(ancestor: DafnyTaxon): bool
}

class {:extern} DafnyObservation {
    function {:extern} getTaxon(): DafnyTaxon
}