module Infrastructure.ScheduleUpdateService

open System.Diagnostics
open FSharp.Collections.ParallelSeq

open System.Collections.Generic
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

    let identitiesFromDb = new List<Identity>(ctx.Identites)
    let newGroups = new List<Identity>()
    
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
    |> PSeq.map (fun (teacherId, groupName, eventType, subjectId) ->
        printfn "Processing data %A" (teacherId, groupName, eventType, subjectId)
        let groupExistInDb = identitiesFromDb |> PSeq.exists (fun identity -> identity.Name = groupName)
        let groupId =
            // If not exists in db, then it is alternative group, let's add it
            if not groupExistInDb then
                let altGroupId = groupName.GetHashCode() |> int64
                let altGroup = { Id = altGroupId; Name = groupName; Type = IdentityType.AlternativeGroup }
                newGroups.Add(altGroup)
                identitiesFromDb.Add(altGroup)
                altGroupId
            else
                (identitiesFromDb |> PSeq.find (fun identity -> identity.Name = groupName)).Id

        { TeacherId = teacherId;
          GroupId = groupId;
          SubjectId = subjectId;
          EventType = LanguagePrimitives.EnumToValue eventType })
    |> refreshDbSet ctx ctx.Teachers
    
    ctx.Identites.AddRange(newGroups)
    ctx.SaveChanges() |> ignore
    
    tx.Commit()

    watch.Stop()
    printfn "Done subject updating in %A" watch.Elapsed

let refreshSubjectsForAllGroups (ctx : ScheduleContext) () =
    ctx.Identites
    |> PSeq.filter (fun identity -> identity.Type = IdentityType.Group)
    |> PSeq.map (fun identity -> identity.Id)
    |> PSeq.toList
    |> refreshSubjectsForGroups ctx