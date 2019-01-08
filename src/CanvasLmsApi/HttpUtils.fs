module CanvasLmsApi.HttpUtils

open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open CanvasLmsApi

// https://www.newtonsoft.com/json/help/html/NamingStrategySnakeCase.htm
let private settings =
    JsonSerializerSettings(
        ContractResolver = DefaultContractResolver(
            NamingStrategy = SnakeCaseNamingStrategy()))
            
let private appendAccessToken (accessToken: string) (url: string) =
    let delimiter =
        match url.Contains("?") with
        | false -> '?'
        | true -> '&'
    sprintf "%s%caccess_token=%s" url delimiter accessToken
    
// https://canvas.instructure.com/doc/api/file.pagination.html
let private tryGetNextLink (accessToken: string) (response: HttpResponse) =
    match response.Headers.ContainsKey("Link") with
    | false -> None
    | true ->
        response.Headers.["Link"]
        |> fun h -> h.Split([| ',' |])
        |> Seq.tryFind(fun s -> s.Contains("rel=\"next\""))
        |> Option.map(fun s -> s.Split([| ';' |]).[0].Replace("<", "").Replace(">", ""))
        |> Option.map(appendAccessToken accessToken)

let rec private getAllLinkedItems<'T> (accessToken: string) (queryParams: list<string * string>) link (prev: 'T list) =
    let response = Http.Request(link, query = queryParams)
    let items =
        match response.Body with
        | Binary(_) -> failwith "Binary responses are not handled"
        | Text(responseString) ->
            JsonConvert.DeserializeObject<'T list>(responseString, settings)
            |> List.append prev
    match tryGetNextLink accessToken response with
    | None -> items
    | Some(nextLink) -> getAllLinkedItems<'T> accessToken queryParams nextLink items

let GetAll<'T> (methodCall: CanvasMethodCall) =
    let link = methodCall.GetUrlString()
    getAllLinkedItems<'T> methodCall.AccessToken (methodCall.GetQueryParameters()) link []
    |> List.toArray

let Post<'T> (methodCall: CanvasMethodCall) =
    let response = Http.Request(methodCall.GetUrlString(), body = FormValues(methodCall.GetQueryParameters()))
    match response.Body with
    | Binary(_) -> failwith "Binary responses are not handled"
    | Text(responseString) -> JsonConvert.DeserializeObject<'T>(responseString)