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
        
        let teacherGroupEventTypeSubject =
            subjects
            |> PSeq.map (fun subject -> subject.TeachersByGroup |> PSeq.map (fun teacherGroup ->
                (teacherGroup.Teacher, teacherGroup.Group, teacherGroup.EventType, subject.Id)))
            |> PSeq.concat
            |> PSeq.distinct
            |> PSeq.toList
        
        let missing = teacherGroupEventTypeSubject
                      |> Seq.map (fun (teacherName, _, _, _) ->
                          let count = context.Identites |> Seq.filter (fun identity -> identity.ShortName = teacherName) |> Seq.length
                          (teacherName, count))
                      |> PSeq.filter (fun (_, count) -> count = 0)
                      |> PSeq.toList
        
        let teacherEntities =
            teacherGroupEventTypeSubject
            |> PSeq.map (fun (teacherName, groupName, eventType, subjectId) ->
                let a = context.Identites |> Seq.filter (fun identity -> identity.ShortName = teacherName) |> Seq.length
                let teacherId = (context.Identites |> Seq.find (fun identity -> identity.ShortName = teacherName)).Id
                let groupId = (context.Identites |> Seq.find (fun identity -> identity.ShortName = groupName)).Id
                { TeacherId = teacherId;
                  GroupId = groupId;
                  SubjectId = subjectId;
                  EventType = LanguagePrimitives.EnumToValue eventType })
            |> PSeq.toList
        
//        let teachersEntities =
//            query {
//                for teachePerGroup in teachersPerGroup do
//                join teacherIdentity in (context.Identites |> Seq.filter (fun i -> i.Type = IdentityType.Teacher))
//                    on (teachePerGroup.Teacher = teacherIdentity.ShortName)
//                join groupIdentity in (context.Identites |> Seq.filter (fun i -> i.Type = IdentityType.Group))
//                    on (teachePerGroup.Group = groupIdentity.ShortName)
//                select ( teacherIdentity.ShortName, groupIdentity.ShortName )
//            }
//            |> PSeq.toList
//            |> PSeq.distinct
//            |> PSeq.toList
        
//        context.Teachers.RemoveRange(context.Teachers)
//        context.Teachers.AddRange(teachersEntities)
//        context.SaveChanges() |> ignore
        
        tx.Commit()
        
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
