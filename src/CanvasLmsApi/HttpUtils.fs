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

let GetAll<'T> (methodCall: CanvasMethodCall) =
    let mutable link = Some(methodCall.GetUrlString())
    seq {
        while link.IsSome do
            let response = Http.Request(link.Value, query = methodCall.GetQueryParameters())
            match response.Body with
            | Text(responseString) -> yield! JsonConvert.DeserializeObject<'T seq>(responseString, settings)
            | Binary(_) -> failwith "Binary responses are not handled"
            link <- tryGetNextLink methodCall.AccessToken response
    }
    |> Seq.cache

let GetSingle<'T> (methodCall: CanvasMethodCall) =
    GetAll<'T>(methodCall) |> Seq.tryHead

let Post<'T> (methodCall: CanvasMethodCall) =
    let response = Http.Request(methodCall.GetUrlString(), body = FormValues(methodCall.GetQueryParameters()))
    match response.Body with
    | Binary(_) -> failwith "Binary responses are not handled"
    | Text(responseString) -> JsonConvert.DeserializeObject<'T>(responseString)