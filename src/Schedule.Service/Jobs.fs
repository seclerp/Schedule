module Service.Jobs

open System
open System.IO
open System.Net
open System.Text

let private apiRoot = "http://cist.nure.ua/ias/app/tt/"

let private requestApiAsync (method : string) (parameters : (string * obj) list ) = async {
    let fullUrl = sprintf "%s%s?%s" apiRoot method (String.Join('&', (parameters |> Seq.map (fun (f, s) -> f + "=" + s.ToString()))))
    let req = WebRequest.CreateHttp fullUrl
    let! res = req.AsyncGetResponse()
    use stream = res.GetResponseStream()
    use reader = new StreamReader(stream, Encoding.GetEncoding("1251"))
    let! data = reader.ReadToEndAsync() |> Async.AwaitTask
    return data
}

let getGroupIdentitiesAsync () = requestApiAsync "P_API_GROUP_JSON" []
let getTeachersIdentitiesAsync () = requestApiAsync "P_API_PODR_JSON" []
let getAuditoriesIdentitiesAsync () = requestApiAsync "P_API_AUDITORIES_JSON" []

let fetchEventsAsync identityId typeId =
    requestApiAsync "P_API_EVENT_JSON" [("timetable_id", identityId); ("type_id", typeId)]
    



//let fetchIdentities = async {
//}