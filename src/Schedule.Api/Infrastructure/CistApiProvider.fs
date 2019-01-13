module Infrastructure.CistApiProvider

open System
open System.Collections.Generic
open System.IO
open System.Net.Http
open System.Text
open System.Text.RegularExpressions
open FSharp.Data
open Domain.Models
open FSharp.Collections.ParallelSeq
open Infrastructure.Models

module private Urls =
    let groupsSchedule  = "http://cist.nure.ua/ias/app/tt/f?p=778:2"
    let facultyGroups   = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_GROUPS"
    let facultyTeachers = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_TEACHS"
    let subjects        = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_GNR_RASP.GEN_GROUP_POTOK_RASP"

module private Patterns =
    let facultyOnClick  = @"javascript:IAS_Change_Groups\(([0-9]+)\)"
    let deptOnClick     = @"javascript:IAS_Change_Kaf\(([0-9]+),([0-9]+)\)"
    let groupOnClick    = @"javascript:IAS_ADD_Group_in_List\('(.+)',([0-9]+)\)"
    let teacherOnClick  = @"javascript:IAS_ADD_Teach_in_List\('(.+)',([0-9]+)\)"
    let eventType       = @"(?:.*: )?([а-яА-ЯҐЄІЇґєії]+) \(\d+\)"

let private validHtmlTemplate =
    sprintf "<html lang=\"ru\" xmlns:htmldb=\"http://htmldb.oracle.com\"><body>%s</body></html>"

let private windows1251 = Encoding.GetEncoding("windows-1251")

let private toStream (inputString : string) = inputString |> Encoding.UTF8.GetBytes |> MemoryStream
let private docToNode (doc : HtmlDocument) = doc.Html()

let private loadHtmlStream (stream : Stream) = stream |> HtmlDocument.Load |> docToNode
let private loadHtml : string -> HtmlNode = toStream >> loadHtmlStream

let inline private cssSelect selector (node : HtmlNode) = node.CssSelect(selector)

let private request url queryParams =
    let uri = if List.isEmpty queryParams then url
              else url + "?" + String.Join("&", (queryParams |> Seq.map (fun (k, v) -> k + "=" + v)))
              |> Uri
    let httpClient = new HttpClient()
    let reponseStream = httpClient.GetStreamAsync(uri)
                        |> Async.AwaitTask
                        |> Async.RunSynchronously

    let reader = new StreamReader(reponseStream, windows1251)
    reader.ReadToEnd()

let private makeValidHtmlStream nonValidHtmlString =
    let valid = nonValidHtmlString |> validHtmlTemplate
    new MemoryStream(Encoding.UTF8.GetBytes(valid))

let private getFaculties () =
    request Urls.groupsSchedule []
    |> loadHtml
    |> cssSelect ".htmldbTabbedNavigationList a"
    |> PSeq.map (fun el -> el.AttributeValue "onclick")
    |> PSeq.filter (fun x -> Regex.IsMatch(x, Patterns.facultyOnClick))
    |> PSeq.map (fun x -> Regex.Match(x, Patterns.facultyOnClick).Groups.[1].Value)

let private createIdentity id name iType =
    { Id = id
      Name = name
      Type = iType
      IsAlternative = Subjects.isAlternative name }

let private getGroupIdentities () =
    getFaculties ()
    |> PSeq.map (fun id ->
        [("p_id_fac", id)]
        |> request Urls.facultyGroups
        |> makeValidHtmlStream
        |> loadHtmlStream
        |> cssSelect ".t13RegionBody a"
        |> PSeq.map (fun el -> el.AttributeValue "onclick")
        |> PSeq.filter (fun x -> Regex.IsMatch(x, Patterns.groupOnClick))
        |> PSeq.map (fun matchedString ->
            let groups = Regex.Match(matchedString, Patterns.groupOnClick).Groups
            createIdentity (groups.[2].Value |> int64) groups.[1].Value IdentityType.Group
        )
    )
    |> PSeq.concat          

let private getDeptsOnFaculties facultyId =
    [("p_id_fac", facultyId); ("p_id_kaf", "1")]
    |> request Urls.facultyTeachers
    |> makeValidHtmlStream
    |> loadHtmlStream
    |> cssSelect ".htmldbTabbedNavigationList"
    |> List.last
    |> cssSelect "a"
    |> PSeq.map (fun el -> el.AttributeValue "onclick")
    |> PSeq.filter (fun x -> Regex.IsMatch(x, Patterns.deptOnClick))
    |> PSeq.map (fun x -> (facultyId, Regex.Match(x, Patterns.deptOnClick).Groups.[2].Value))

// Sometimes teacher first name is separated from last and sur names with two spaces, e.g.:
// "Ivanov  I. I." instead of
// "Ivanov I. I."
// To fix this - we replace "  " with " "
let private fixTeacherName (teacherName : string) = teacherName.Replace("  ", " ")

let private getTeacherIdentities () =
    getFaculties ()
    |> PSeq.map getDeptsOnFaculties
    |> PSeq.concat
    |> PSeq.map (fun (fac, dept) ->
        [("p_id_fac", fac); ("p_id_kaf", dept)]
        |> request Urls.facultyTeachers 
        |> makeValidHtmlStream
        |> loadHtmlStream
        |> cssSelect ".t13datatop a"
        |> PSeq.map (fun el -> el.AttributeValue "onclick")
        |> PSeq.filter (fun x -> Regex.IsMatch(x, Patterns.teacherOnClick))
        |> PSeq.map (fun matched ->
            let groups = Regex.Match(matched, Patterns.teacherOnClick).Groups
            let fixedName = fixTeacherName groups.[1].Value
            createIdentity (groups.[2].Value |> int64) fixedName IdentityType.Teacher
        )
    )
    |> PSeq.concat

let getAllIdentities () =
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

let private parseComplexGroup (complexGroup : string) =
    let processOne =
        function
        | x when Subjects.isAlternative x ->
            [ x.Replace("-)", ")") ]
        | x when x.Contains(",") ->
            let commaSeparated = x.Split(",")
            let mainGroupParts =  commaSeparated.[0].Split("-")
            let baseHead = String.Join("-", mainGroupParts.[..mainGroupParts.Length - 2])
            [ commaSeparated.[0] ] @ (commaSeparated.[1..] |> PSeq.map (fun part -> baseHead + "-" + part) |> PSeq.toList)
        | x -> [ x ]

    let group = if complexGroup.EndsWith(", ") then complexGroup.Substring(0, complexGroup.Length - 2) else complexGroup
    let result = group.Replace(" ", "").Split(";")
                 |> PSeq.map processOne
                 |> PSeq.concat
                 |> PSeq.toList
    result

type private TeachersParserState = {
    CurrentType     : EventType
    CurrentGroups   : string list
    CurrentTeachers : string list
    Result          : TeachersPerGroup list
}
let private getDefaultParserState () =
    { CurrentType = EventType.Lecture
      CurrentGroups = []
      CurrentTeachers = []
      Result = [] }

let makeTeacherForGroup result eventType teacher group =
    { Teacher = teacher; EventType = eventType; Group = group }

type private ParserToken =
    | GroupName of string
    | TeacherName of string
    | Text of string

let rec private getTokensFromRowsRec (tokens : ParserToken list) (rows : HtmlNode list) =
    match rows with
    // Parse group names
    | x::xs when x.AttributeValue("href").Contains("_GROUP") ->
        let groups = x.InnerText().Split(";")
                     |> PSeq.map parseComplexGroup
                     |> PSeq.concat
                     |> PSeq.map GroupName
                     |> PSeq.toList
        getTokensFromRowsRec (tokens @ groups) xs
    // Parse teacher name
    | x::xs when x.AttributeValue("href").Contains("_KAF") ->
        getTokensFromRowsRec (tokens @ [ x.InnerText() |> TeacherName ]) xs
    | [] -> tokens
    // Parse usual text
    | x::xs ->
        // Try to split by ' - ' to process non-linkable groups (alternatives)
        let splitted = x.InnerText().Split(" - ", StringSplitOptions.RemoveEmptyEntries)
        if splitted |> PSeq.length = 2 then
            let text = splitted |> PSeq.head |> Text
            let groupNames = splitted |> Seq.last |> parseComplexGroup |> PSeq.map GroupName |> PSeq.toList
            getTokensFromRowsRec (tokens @ [ text ] @ groupNames) xs
        else
            getTokensFromRowsRec (tokens @ [ x.InnerText() |> Text ]) xs
            
let private getTokensFromRows =
    getTokensFromRowsRec []

let rec private parseTeachersRec state =
    function
    | (GroupName group)::tail ->
        parseTeachersRec { state with CurrentGroups = state.CurrentGroups @ [ group ] } tail
    | (TeacherName teacher)::tail ->
        let newResult = state.Result @ (state.CurrentGroups
                                        |> PSeq.map (makeTeacherForGroup state.Result state.CurrentType teacher)
                                        |> PSeq.toList)
        parseTeachersRec { state with Result = newResult; CurrentTeachers = state.CurrentTeachers @ [ teacher ] } tail
    | (Text text)::tail when Regex.IsMatch(text, Patterns.eventType) ->
        let regMatch = Regex.Match(text, Patterns.eventType)
        let newType = mapEventType regMatch.Groups.[1].Value
        parseTeachersRec { state with CurrentType = newType; CurrentGroups = []; CurrentTeachers = [] } tail
    | [] -> state.Result
    | x::xs -> parseTeachersRec state xs

let private parseTeachers (infoTableRow : HtmlNode) =
    let tokens = 
        infoTableRow.Elements()
        |> getTokensFromRows
    tokens |> parseTeachersRec (getDefaultParserState ())

let getAllSubjects : long list -> SubjectModel list =
    Seq.chunkBySize 50
    // Do not make Seq.map parallel - CIST can't process 2 or more requests per time
    >> Seq.map (fun (idsChunk : long array) ->
        printfn "Start proecessing for IDs: %s" (String.Join(", ", idsChunk))
        let semesterBounds = getSemesterBounds DateTime.Now
        let groupsConcated = String.Join("_", idsChunk)
        let dateStart = (fst semesterBounds).ToString("dd.MM.yyyy")
        let dateEnd = (snd semesterBounds).ToString("dd.MM.yyyy")
        [ ("ATypeDoc", "1")
          ("Aid_group", groupsConcated)
          ("Aid_potok", "0")
          ("ADateStart", dateStart)
          ("ADateEnd", dateEnd) ]
        |> request Urls.subjects
        |> makeValidHtmlStream
        |> loadHtmlStream
        |> cssSelect "table.footer tr"
        |> PSeq.map (fun tr ->
            let nameTableRow = tr |> cssSelect "td[class=name]" |> PSeq.head
            let infoTableRow = tr |> cssSelect "td" |> PSeq.filter (fun td -> td <> nameTableRow) |> PSeq.head
            let subjectInfoLink = nameTableRow.Elements() |> PSeq.head
        
            let subjectId = subjectInfoLink.AttributeValue "name" |> int64
            let subjectBrief = subjectInfoLink.InnerText()
            let subjectTitle = infoTableRow.InnerText().Split(':') |> PSeq.head
        
            { Id = subjectId;
              Brief = subjectBrief;
              Title = subjectTitle;
              TeachersByGroup = parseTeachers infoTableRow }
        )
    )
    >> PSeq.concat
    >> PSeq.distinct
    >> PSeq.sortBy (fun subject -> subject.Id)
    >> PSeq.toList

// long -> long -> Event list
let getSchedule identityId identityType = ()