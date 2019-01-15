module Infrastructure.ScheduleUpdateService

open System.Diagnostics
open FSharp.Collections.ParallelSeq

open Domain.Models
open Domain.ScheduleContext
open Infrastructure
open Microsoft.EntityFrameworkCore

let private refreshDbSet (ctx : ScheduleContext) (dbSet : DbSet<'a>) (newData : seq<'a>) =
    dbSet.RemoveRange(dbSet)
    ctx.SaveChanges() |> ignore
    dbSet.AddRange(newData)
    ctx.SaveChanges() |> ignore

let refreshIdentities (ctx : ScheduleContext) () =
    let watch = Stopwatch()
    watch.Start()
    CistApiProvider.getAllIdentities()
    |> refreshDbSet ctx ctx.Identites
    watch.Stop()
    printfn "Done in %A" watch.Elapsed

let refreshSubjectsForGroups (ctx : ScheduleContext) groupIdentityIds =
    let watch = Stopwatch()
    watch.Start()
    
    use tx = ctx.Database.BeginTransaction()
    
    let subjects = CistApiProvider.getAllSubjects groupIdentityIds |> PSeq.distinct
    
    subjects
    |> PSeq.map (fun subject -> { Id = subject.Id; Brief = subject.Brief; Title = subject.Title })
    |> PSeq.distinct
    |> refreshDbSet ctx ctx.Subjects
    
    subjects
    |> PSeq.map (fun subject -> subject.TeachersByGroup
                             |> PSeq.map (fun tpg -> (tpg.Teacher, tpg.Group, tpg.EventType, subject.Id))
                             |> PSeq.toList)
    |> PSeq.toList
    |> List.concat
    |> List.distinct
    |> List.map (fun (teacherName, groupName, eventType, subjectId) ->
        printfn "Processing data %A" (teacherName, groupName, eventType, subjectId)
        let groupExistInDb = ctx.Identites |> PSeq.exists (fun identity -> identity.Name = groupName)
        let groupId =
            // If not exists in db, then it is alternative group, let's add it
            if not groupExistInDb then
                let altGroupId = groupName.GetHashCode() |> int64
                ctx.Identites.Add({ Id = altGroupId; Name = groupName; Type = IdentityType.AlternativeGroup }) |> ignore
                ctx.SaveChanges() |> ignore
                altGroupId
            else
                (ctx.Identites |> PSeq.find (fun identity -> identity.Name = groupName)).Id

        { TeacherId = (ctx.Identites |> PSeq.find (fun identity -> identity.Name = teacherName)).Id;
          GroupId = groupId;
          SubjectId = subjectId;
          EventType = LanguagePrimitives.EnumToValue eventType })
    |> refreshDbSet ctx ctx.Teachers
    
    tx.Commit()
    
    watch.Stop()
    printfn "Done subject updating in %A" watch.Elapsed
    
let refreshSubjectsForAllGroups (ctx : ScheduleContext) () =
    ctx.Identites
    |> PSeq.filter (fun identity -> identity.Type = IdentityType.Group)
    |> PSeq.map (fun identity -> identity.Id)
    |> PSeq.toList
    |> refreshSubjectsForGroups ctx