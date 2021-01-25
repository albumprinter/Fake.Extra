namespace Albelli

    module Seq =
        let zipWithInfinite (prod: int -> 'b) (seq : seq<'a> ) =
            Seq.initInfinite prod
            |> Seq.zip seq