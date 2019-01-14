namespace src.Controllers

open Microsoft.AspNetCore.Mvc
open System.Diagnostics
open FSharp.Collections.ParallelSeq

open System
open Domain.Models
open Domain.ScheduleContext
open Infrastructure.CistApiProvider

[<Route("api/admin")>]
[<ApiController>]
type AdminController (context : ScheduleContext) =
    inherit ControllerBase()

    [<HttpPost("identities/update")>]
    member this.UpdateIdentities() = 
        
        
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
                          let count = context.Identites |> Seq.filter (fun identity -> identity.Name = teacherName) |> Seq.length
                          (teacherName, count))
                      |> PSeq.filter (fun (_, count) -> count = 0)
                      |> PSeq.toList
        
        let teachersEntities =
            teacherGroupEventTypeSubject
            |> Seq.map (fun (teacherName, groupName, eventType, subjectId) ->
                let groupExistInDb = context.Identites |> PSeq.exists (fun identity -> identity.Name = groupName)
                let groupId =
                    // If not exists in db, then it is alternative group, let's add it
                    if not groupExistInDb then
                        let altGroupId = groupName.GetHashCode() + Int32.MaxValue |> int64
                        context.Identites.Add(
                            { Id = altGroupId;
                              Name = groupName;
                              Type = IdentityType.AlternativeGroup }
                        ) |> ignore
                        context.SaveChanges() |> ignore
                        altGroupId
                    else
                        (context.Identites |> PSeq.find (fun identity -> identity.Name = groupName)).Id

                let teacherId = (context.Identites |> PSeq.find (fun identity -> identity.Name = teacherName)).Id
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
        
        context.Teachers.RemoveRange(context.Teachers)
        context.Teachers.AddRange(teachersEntities)
        context.SaveChanges() |> ignore
        
        tx.Commit()
        
        watch.Stop()
        printfn "Done in %A" watch.Elapsed
