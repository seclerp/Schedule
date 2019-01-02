module Program

open System.Text
open Service

[<EntryPoint>]
let main argv =
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    
    Jobs.getTeachersIdentitiesAsync()
    |> Async.RunSynchronously
    |> printfn "%A"
    0