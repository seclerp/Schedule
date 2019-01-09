namespace src.Controllers

open Microsoft.AspNetCore.Mvc
open System.Diagnostics

open System
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
//                                       |> PSeq.filter (fun identity -> identity.Type = IdentityType.Group)
//                                       |> PSeq.map (fun identity -> identity.Id)
//                                       |> PSeq.toList)
        let subjectEntities = subjects
                              |> PSeq.map (fun subject ->
                                  { Id = subject.Id; Brief = subject.Brief; Title = subject.Title })
                              |> PSeq.toArray
        use tx = context.Database.BeginTransaction()
        
        context.Subjects.RemoveRange(context.Subjects)
        context.Subjects.AddRange(subjectEntities)
        context.SaveChanges() |> ignore
        
        let teachersEntities =
            subjects
            |> Seq.map (fun subject ->
                query {
                    for teacherByGroup in subject.TeachersByGroup do
                    join teacherIdentity in context.Identites on (teacherByGroup.Teacher = teacherIdentity.ShortName)
                    join groupIdentity in context.Identites on (teacherByGroup.Group = groupIdentity.ShortName)
                    select { EntryId = Guid.NewGuid()
                             TeacherId = teacherIdentity.Id;
                             GroupId = groupIdentity.Id;
                             SubjectId = subject.Id;
                             EventType = LanguagePrimitives.EnumToValue teacherByGroup.EventType }
                } |> Seq.toList)
            |> PSeq.concat
            |> PSeq.toList
        
        context.Teachers.RemoveRange(context.Teachers)
        context.Teachers.AddRange(teachersEntities)
        context.SaveChanges() |> ignore
        
        tx.Commit()
        
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
