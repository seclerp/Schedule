module Infrastructure.ScheduleUpdateService
open Domain.ScheduleContext

let refreshIdentities (ctx : ScheduleContext) =
    let watch = Stopwatch()
    watch.Start()
    let identities = getAllIdentities()
    context.Identites.RemoveRange(context.Identites)
    context.Identites.AddRange(identities)
    context.SaveChanges() |> ignore
    watch.Stop()
    printfn "Done in %A" watch.Elapsed