module CsvApiProvider

open System.IO
open System.Text
open System.Text.RegularExpressions
open FSharp.Data
open Domain.Models
open FSharp.Collections.ParallelSeq

let htmlCistScheduleGroupsUrl   = "http://cist.nure.ua/ias/app/tt/f?p=778:2:7630337726462775"
let htmlCistScheduleTeachersUrl = "http://cist.nure.ua/ias/app/tt/f?p=778:4:7630337726462775"
let htmlFacultyGroupsUrl        = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_GROUPS"
let htmlFacultyTeachersUrl      = "http://cist.nure.ua/ias/app/tt/WEB_IAS_TT_AJX_TEACHS"

let idFacultiesPattern = @"javascript:IAS_Change_Groups\(([0-9]+)\)"
let idKafPattern       = @"javascript:IAS_Change_Kaf\(([0-9]+),([0-9]+)\)"
let idGroupPattern     = @"javascript:IAS_ADD_Group_in_List\('(.+)',([0-9]+)\)"
let idTeacherPattern   = @"javascript:IAS_ADD_Teach_in_List\('(.+)',([0-9]+)\)"

let windows1251 = Encoding.GetEncoding("windows-1251")

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
        let nonValid = Http.RequestString(htmlFacultyGroupsUrl + "?p_id_fac=" + id)
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
    let nonValid = Http.RequestString(htmlFacultyTeachersUrl + "?p_id_fac=" + facultyId + "&p_id_kaf=-1")
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
        let nonValid = Http.RequestString(htmlFacultyTeachersUrl + "?p_id_fac=" + fac + "&p_id_kaf=" + kaf)
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

// unit -> Identity list
let getIdentities () =
    [ getGroupIdentities(); getTeacherIdentities(); ]
    |> PSeq.concat
    |> PSeq.distinctBy (fun identity -> identity.Id)
    |> PSeq.sortBy (fun identity -> identity.Id)
    |> PSeq.toList

// long -> long -> Event list
let getSchedule identityId identityType = ()