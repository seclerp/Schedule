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
    
    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = refreshIdentities ()

    [<HttpPost("subjects/update/{id}")>]
    member this.UpdateSubjects(id : long) = refreshSubjectsForGroups [id]
    
    [<HttpPost("subjects/update")>]
    member this.UpdateSubjects([<FromBody>] ids : long list) = refreshSubjectsForGroups ids
    
    [<HttpPost("subjects/update/all")>]
    member this.UpdateSubjects() = refreshSubjectsForAllGroups ()