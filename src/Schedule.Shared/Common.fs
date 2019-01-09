[<AutoOpen>]
module Common

type long = int64

module Subjects =
    let isAlternative (name : string) = name.StartsWith("*")