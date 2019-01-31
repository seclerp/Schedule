module Infrastructure.CsvParser

open System

let private itemPattern = "[^,]+"

let getValue (row : Map<string, string>) (column : string) =
    match row.TryGetValue column with
    | (true, value) -> value
    | (false, _) -> failwithf "Column with name %s not found" column

let private extractStringFromLiteral (str : string) =
    if str.StartsWith("\"") then str.[1..]
    else if str.EndsWith("\"") then str.[..str.Length-2]
    else str

let private processRow (rowString : string) =
    rowString.Trim().Split("\",\"")
    |> Seq.map extractStringFromLiteral
    |> Seq.toList

let parse (input : string) =
    let lines = input.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries)
    if Array.length lines = 0 then []
    else lines.[1..] |> Seq.map (processRow >> Seq.zip (processRow lines.[0]) >> Map.ofSeq) |> Seq.toList