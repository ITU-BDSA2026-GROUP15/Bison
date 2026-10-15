//Wrapper.cs generated automatically with command -> dafny translate cs Wrapper.dfy

// Dafny program Wrapper.dfy compiled into C#
// To recompile, you will need the libraries
//     System.Runtime.Numerics.dll System.Collections.Immutable.dll
// but the 'dotnet' tool in .NET should pick those up automatically.
// Optionally, you may want to include compiler switches like
//     /debug /nowarn:162,164,168,183,219,436,1717,1718

using System;
using System.Numerics;
using System.Collections;
[assembly: DafnyAssembly.DafnySourceAttribute(@"// dafny 4.11.0.0
// Command-line arguments: translate cs Wrapper.dfy
// Wrapper.dfy


module {:extern ""Bison.Dafny.wrapper""} Wrapper {

  import opened Models

  import Filtering
  class Wrap {
    static method Filter(root: Taxon, obs: seq<Observation>) returns (r: seq<Observation>)
      decreases root, obs
    {
      r := Filtering.FilterBy(root, obs);
    }
  }
}

module Filtering {
  function FilterBy(root: Taxon, obs: seq<Observation>): seq<Observation>
    ensures forall o: Observation {:trigger o.getTaxon()} {:trigger o in obs} {:trigger o in FilterBy(root, obs)} :: (o in FilterBy(root, obs) ==> o in obs) && (o in FilterBy(root, obs) ==> o.getTaxon().isSubTaxon(root))
    ensures forall o: Observation {:trigger o in FilterBy(root, obs)} {:trigger o.getTaxon()} {:trigger o in obs} :: o in obs && o.getTaxon().isSubTaxon(root) ==> o in FilterBy(root, obs)
    ensures |FilterBy(root, obs)| <= |obs|
    decreases root, obs
  {
    if |obs| == 0 then
      []
    else
      (if obs[0].getTaxon().isSubTaxon(root) then [obs[0]] else []) + FilterBy(root, obs[1..])
  }

  import opened Models
}

module Models {
  class {:extern ""Bison.Razor.Models.DafnyTaxon""} Taxon {
    function {:extern} {:axiom} isSubTaxon(ancestor: Taxon): bool
      decreases ancestor
  }

  class {:extern ""Bison.Razor.Models.DafnyObservation""} Observation {
    function {:extern} {:axiom} getTaxon(): Taxon
  }
}
")]

namespace Dafny {
  internal class ArrayHelpers {
    public static T[] InitNewArray1<T>(T z, BigInteger size0) {
      int s0 = (int)size0;
      T[] a = new T[s0];
      for (int i0 = 0; i0 < s0; i0++) {
        a[i0] = z;
      }
      return a;
    }
  }
} // end of namespace Dafny
internal static class FuncExtensions {
  public static Func<UResult> DowncastClone<TResult, UResult>(this Func<TResult> F, Func<TResult, UResult> ResConv) {
    return () => ResConv(F());
  }
  public static Func<U, UResult> DowncastClone<T, TResult, U, UResult>(this Func<T, TResult> F, Func<U, T> ArgConv, Func<TResult, UResult> ResConv) {
    return arg => ResConv(F(ArgConv(arg)));
  }
  public static Func<U1, U2, UResult> DowncastClone<T1, T2, TResult, U1, U2, UResult>(this Func<T1, T2, TResult> F, Func<U1, T1> ArgConv1, Func<U2, T2> ArgConv2, Func<TResult, UResult> ResConv) {
    return (arg1, arg2) => ResConv(F(ArgConv1(arg1), ArgConv2(arg2)));
  }
}
// end of class FuncExtensions
namespace Models {



} // end of namespace Models
namespace Filtering {

  public partial class __default {
    public static Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> FilterBy(Models.Bison.Razor.Models.DafnyTaxon root, Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> obs)
    {
      Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> _0___accumulator = Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.FromElements();
    TAIL_CALL_START: ;
      if ((new BigInteger((obs).Count)).Sign == 0) {
        return Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.Concat(_0___accumulator, Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.FromElements());
      } else {
        _0___accumulator = Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.Concat(_0___accumulator, (((((obs).Select(BigInteger.Zero)).getTaxon()).isSubTaxon(root)) ? (Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.FromElements((obs).Select(BigInteger.Zero))) : (Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.FromElements())));
        Models.Bison.Razor.Models.DafnyTaxon _in0 = root;
        Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> _in1 = (obs).Drop(BigInteger.One);
        root = _in0;
        obs = _in1;
        goto TAIL_CALL_START;
      }
    }
  }
} // end of namespace Filtering
namespace Bison.Dafny.wrapper {


  public partial class Wrap {
    public Wrap() {
    }
    public static Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> Filter(Models.Bison.Razor.Models.DafnyTaxon root, Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> obs)
    {
      Dafny.ISequence<Models.Bison.Razor.Models.DafnyObservation> r = Dafny.Sequence<Models.Bison.Razor.Models.DafnyObservation>.Empty;
      r = Filtering.__default.FilterBy(root, obs);
      return r;
    }
  }
} // end of namespace Bison.Dafny.wrapper
namespace _module {

} // end of namespace _module
