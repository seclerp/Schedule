module Controllers.FetchIdentities

open System.Diagnostics
open Domain.ScheduleContext
open Microsoft.AspNetCore.Mvc

[<Route("api/[controller]")>]
[<ApiController>]
type AdminController (context : ScheduleContext) =
    inherit ControllerBase()

    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = 
        let watch = Stopwatch()
        watch.Start()
        let identities = CsvApiProvider.getIdentities()
        context.Identites.RemoveRange(context.Identites)
        context.Identites.AddRange()
        context.SaveChanges() |> ignore
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
