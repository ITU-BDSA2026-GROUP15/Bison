include "Filter.dfy"
// include "dafnyModels.dfy" -> implicit through the Filtering Module

//the extern name is the namespace we connect via in c#
module {:extern "DafnyWrapper"} Wrapper {
    import opened Models
    import Filtering //when keyword opened is not used we still have to use Filtering.FilterBy

    //Dafny demands the methods to be in a class to be be called from c#
    class Wrap {
        static method Filter(root: Taxon, obs: seq<Observation>)
        returns (r: seq<Observation>)
        {
            //calls the method for verifying the filters
            r := Filtering.FilterBy(root, obs);
        }
    }
}