module Infrastructure.CsvApiProvider

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Text
open System.Text.RegularExpressions
open FSharp.Data
open Domain.Models
open FSharp.Collections.ParallelSeq
open Infrastructure.Models

let htmlCistScheduleGroupsUrl   = "http://cist.nure.ua/ias/app/tt/f?p=778:2:7630337726462775"
let htmlCistScheduleTeachersUrl = "http://cist.nure.ua/ias/app/tt/f?p=778:4:7630337726462775"
let htmlFacultyGroupsUrl        = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_GROUPS"
let htmlFacultyTeachersUrl      = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_TEACHS"
let htmlSubjectsUrl             = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_GNR_RASP.GEN_GROUP_POTOK_RASP"

let idFacultiesPattern = @"javascript:IAS_Change_Groups\(([0-9]+)\)"
let idKafPattern       = @"javascript:IAS_Change_Kaf\(([0-9]+),([0-9]+)\)"
let idGroupPattern     = @"javascript:IAS_ADD_Group_in_List\('(.+)',([0-9]+)\)"
let idTeacherPattern   = @"javascript:IAS_ADD_Teach_in_List\('(.+)',([0-9]+)\)"
let subjectIdPattern   = @".*: ([а-яА-Я]+) \(\d+\)"

let windows1251 = Encoding.GetEncoding("windows-1251")

let private request (url : string) (queryParams : (string * string) list) =
    let uri = if List.isEmpty queryParams then url
              else url + "?" + String.Join("&", (queryParams |> Seq.map (fun (k, v) -> k + "=" + v)))
              |> Uri
    let req = HttpWebRequest.Create(uri) :?> HttpWebRequest 
    let resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    let reader = new StreamReader(stream, windows1251) 
    reader.ReadToEnd()

let private makeValidHtmlStream nonValidHtmlString =
    let valid = nonValidHtmlString
                |> sprintf "<html lang=\"ru\" xmlns:htmldb=\"http://htmldb.oracle.com\"><body>%s</body></html>"
    let stream = new MemoryStream(Encoding.UTF8.GetBytes(valid))
    stream

let private getFaculties () =
    HtmlDocument.Load(htmlCistScheduleGroupsUrl, windows1251)
        .CssSelect ".htmldbTabbedNavigationList a"
    |> PSeq.map (fun el -> el.AttributeValue "onclick")
    |> PSeq.filter (fun x -> Regex.IsMatch(x, idFacultiesPattern))
    |> PSeq.map (fun x -> Regex.Match(x, idFacultiesPattern).Groups.[1].Value)

let private getGroupIdentities () =
    getFaculties ()
    |> PSeq.map (fun id ->
        let nonValid = request htmlFacultyGroupsUrl [("p_id_fac", id)]
        let doc = nonValid |> makeValidHtmlStream |> HtmlDocument.Load
        doc.CssSelect ".t13RegionBody a"
        |> PSeq.map (fun el -> el.AttributeValue "onclick")
        |> PSeq.filter (fun x -> Regex.IsMatch(x, idGroupPattern))
        |> PSeq.map (fun x -> Regex.Match(x, idGroupPattern).Groups)
        |> PSeq.map (fun groups -> {
            Id = groups.[2].Value |> int64
            ShortName = groups.[1].Value
            FullName = groups.[1].Value
            Type = IdentityType.Group
        })
    )
    |> PSeq.concat
               

let private getKafsOnFaculties facultyId =
    let nonValid = request htmlFacultyTeachersUrl [("p_id_fac", facultyId); ("p_id_kaf", "1")]
    let kafsNav = (nonValid |> makeValidHtmlStream |> HtmlDocument.Load).CssSelect ".htmldbTabbedNavigationList"
                  |> List.last
    kafsNav.CssSelect "a"
    |> PSeq.map (fun el -> el.AttributeValue "onclick")
    |> PSeq.filter (fun x -> Regex.IsMatch(x, idKafPattern))
    |> PSeq.map (fun x -> (facultyId, Regex.Match(x, idKafPattern).Groups.[2].Value))

let private getTeacherIdentities () =
    getFaculties ()
    |> PSeq.map getKafsOnFaculties
    |> PSeq.concat
    |> PSeq.map (fun (fac, kaf) ->
        let nonValid = request htmlFacultyTeachersUrl [("p_id_fac", fac); ("p_id_kaf", kaf)]
        let doc = nonValid |> makeValidHtmlStream |> HtmlDocument.Load
        doc.CssSelect ".t13datatop a"
        |> PSeq.map (fun el -> el.AttributeValue "onclick")
        |> PSeq.filter (fun x -> Regex.IsMatch(x, idTeacherPattern))
        |> PSeq.map (fun x -> Regex.Match(x, idTeacherPattern).Groups)
        |> PSeq.map (fun groups -> {
            Id = groups.[2].Value |> int64
            ShortName = groups.[1].Value
            FullName = groups.[1].Value
            Type = IdentityType.Teacher
        })
    )
    |> PSeq.concat

let getAllIdentities () : Identity list =
    [ getGroupIdentities(); getTeacherIdentities(); ]
    |> PSeq.concat
    |> PSeq.distinctBy (fun identity -> identity.Id)
    |> PSeq.sortBy (fun identity -> identity.Id)
    |> PSeq.toList

let private getSemesterBounds (currentTime : DateTime) =
    // First part of first semester (09.01 - 01.01)
    if currentTime >= DateTime(currentTime.Year, 9, 1) && currentTime <= DateTime(currentTime.Year + 1, 1, 1) then
        (DateTime(currentTime.Year, 9, 1), DateTime(currentTime.Year + 1, 2, 18))
    // Second part of first semester (01.01 - 18.02)
    else if currentTime > DateTime(currentTime.Year, 1, 1) && currentTime <= DateTime(currentTime.Year + 1, 2, 18) then
        (DateTime(currentTime.Year - 1, 9, 1), DateTime(currentTime.Year, 2, 18))
    // Second semester (19.02 - 18.08)
    else
        (DateTime(currentTime.Year, 2, 19), DateTime(currentTime.Year, 8, 18))

type private TeachersParseState = {
    CurrentType : EventType
    Teachers : TeacherModel list
}

let rec private parseTeachersRec state dict (rows : HtmlNode list) =
    match rows with
    | x::xs when Regex.IsMatch(x.InnerText(), subjectIdPattern) ->
        let regMatch = Regex.Match(x.InnerText(), subjectIdPattern)
        parseTeachersRec { state with CurrentType = EventType.map regMatch.Groups.[1].Value } dict xs
    | x::xs when Regex.IsMatch(x.InnerText(), subjectIdPattern) -> 
    | [] -> dict
    | _::xs -> parseTeachersRec state dict xs
        //new Dictionary<EventType, TeacherModel list>()

let private parseTeachers (infoTableRow : HtmlNode) =
    parseTeachersRec
        { CurrentType = Lecture; Teachers = [] }
        (new Dictionary<EventType, TeacherModel list>())
        (infoTableRow.Elements())

let getAllSubjects : long list -> SubjectModel list =
    Seq.chunkBySize 50
    >> Seq.map (fun (idsChunk : long array) ->
        printfn "Start proecessing for IDs: %s" (String.Join(", ", idsChunk))
        let semesterBounds = getSemesterBounds DateTime.Now
        let groupsConcated = String.Join("_", idsChunk)
        let dateStart = (fst semesterBounds).ToString("dd.MM.yyyy")
        let dateEnd = (snd semesterBounds).ToString("dd.MM.yyyy")
        request htmlSubjectsUrl [
            ("ATypeDoc", "1")
            ("Aid_group", groupsConcated)
            ("Aid_potok", "0")
            ("ADateStart", dateStart)
            ("ADateEnd", dateEnd)
        ]
        |> makeValidHtmlStream
        |> HtmlDocument.Load
        |> (fun doc -> doc.CssSelect "table.footer tr")
        |> PSeq.map (
               fun tr ->
                   let nameTableRow = tr.CssSelect "td[class=name]" |> Seq.head
                   let infoTableRow = tr.CssSelect "td" |> PSeq.filter (fun td -> td <> nameTableRow) |> Seq.head
                   let subjectInfoLink = nameTableRow.Elements() |> Seq.head
                
                   let subjectId = subjectInfoLink.AttributeValue "name" |> int64
                   let subjectBrief = subjectInfoLink.InnerText()
                   let subjectTitle = infoTableRow.InnerText().Split(':') |> Seq.head
                
                   { Id = subjectId;
                     Brief = subjectBrief;
                     Title = subjectTitle;
                     Teachers = parseTeachers infoTableRow }
           )
    )
    >> PSeq.concat
    >> PSeq.distinctBy (fun subject -> subject.Id)
    >> PSeq.sortBy (fun subject -> subject.Id)
    >> PSeq.toList

// long -> long -> Event list
let getSchedule identityId identityType = ()