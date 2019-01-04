namespace src.Controllers

open Microsoft.AspNetCore.Mvc
open System.Diagnostics

open Domain.ScheduleContext
open Infrastructure.CsvApiProvider


[<Route("api/[controller]")>]
[<ApiController>]
type AdminController (context : ScheduleContext) =
    inherit ControllerBase()

    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = 
        let watch = Stopwatch()
        watch.Start()
        let identities = getIdentities()
        context.Identites.RemoveRange(context.Identites)
        context.Identites.AddRange(identities)
        context.SaveChanges() |> ignore
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
