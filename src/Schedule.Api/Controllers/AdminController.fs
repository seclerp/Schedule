namespace src.Controllers

open Microsoft.AspNetCore.Mvc

open Domain.ScheduleContext
open Infrastructure.ScheduleUpdateService

[<Route("api/admin")>]
[<ApiController>]
type AdminController (context : ScheduleContext) =
    inherit ControllerBase()

    let refreshIdentities = refreshIdentities context
    let refreshSubjectsForGroups = refreshSubjectsForGroups context
    let refreshSubjectsForAllGroups = refreshSubjectsForAllGroups context
    let refreshEventsForGroups = refreshEventsForGroups context
    let refreshEventsForAllGroups = refreshEventsForAllGroups context
    
    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = refreshIdentities ()

    [<HttpPost("subjects/update/{id}")>]
    member this.UpdateSubjects(id : long) = refreshSubjectsForGroups [id]
    
    [<HttpPost("subjects/update")>]
    member this.UpdateSubjects([<FromBody>] ids : long list) = refreshSubjectsForGroups ids
    
    [<HttpPost("subjects/update/all")>]
    member this.UpdateSubjects() = refreshSubjectsForAllGroups ()
    
    [<HttpPost("events/update/{id}")>]
    member this.UpdateEvents(id : long) = refreshEventsForGroups [id]
    
    [<HttpPost("events/update/all")>]
    member this.UpdateEvents() = refreshEventsForAllGroups ()