namespace src.Controllers

open Microsoft.AspNetCore.Mvc
open System.Diagnostics

open Domain.Models
open Domain.ScheduleContext
open FSharp.Collections.ParallelSeq
open Infrastructure.CsvApiProvider
open Microsoft.EntityFrameworkCore


[<Route("api/admin")>]
[<ApiController>]
type AdminController (context : ScheduleContext) =
    inherit ControllerBase()

    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = 
        let watch = Stopwatch()
        watch.Start()
        let identities = getAllIdentities()
        context.Identites.RemoveRange(context.Identites)
        context.Identites.AddRange(identities)
        context.SaveChanges() |> ignore
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
        
    [<HttpGet("subjects/update")>]
    member this.UpdateSubjects() = 
        let watch = Stopwatch()
        watch.Start()
        let subjects = getAllSubjects [5721681L]
//        let subjects = getAllSubjects (context.Identites
//                                       |> PSeq.filter (fun i -> i.Type = IdentityType.Group)
//                                       |> PSeq.map (fun i -> i.Id)
//                                       |> Seq.toList)
        //context.Subjects.RemoveRange(context.Subjects)
        //context.Subjects.AddRange(subjects)
        //context.SaveChanges() |> ignore
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
