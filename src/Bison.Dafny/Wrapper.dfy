include "Filter.dfy"
include "dafnyModels.dfy"

module {:extern "Bison.Dafny.wrapper"} Wrapper {
    import opened Models
    import Filtering

    class Wrap {
        static method Filter(root: Taxon, obs: seq<Observation>)
        returns (r: seq<Observation>)
        {
            r := Filtering.FilterBy(root, obs);
        }
    }
}