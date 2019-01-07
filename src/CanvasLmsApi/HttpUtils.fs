module CanvasLmsApi.HttpUtils

open System.Text.RegularExpressions
open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open CanvasLmsApi
open Newtonsoft.Json

// https://www.newtonsoft.com/json/help/html/NamingStrategySnakeCase.htm
let private settings =
    JsonSerializerSettings(
        ContractResolver = DefaultContractResolver(
            NamingStrategy = SnakeCaseNamingStrategy()))
            
// https://canvas.instructure.com/doc/api/file.pagination.html
let private tryGetNextLink (site: CanvasSite) (response: HttpResponse) =
    match response.Headers.ContainsKey("Link") with
    | false -> None
    | true ->
        response.Headers.["Link"]
        |> fun h -> h.Split([| ',' |])
        |> Seq.tryFind(fun s -> s.Contains("rel=\"next\""))
        |> Option.map(fun s -> s.Split([| ';' |]).[0].Replace("<", "").Replace(">", ""))
        |> Option.map(site.AppendAccessToken)

let rec private getAllLinkedItems<'T> (site: CanvasSite) link queryParams (prev: 'T list) =
    let response = Http.Request(link, query = queryParams)
    let items =
        match response.Body with
        | Binary(_) -> failwith "Binary responses are not handled"
        | Text(responseString) ->
            JsonConvert.DeserializeObject<'T list>(responseString, settings)
            |> List.append prev
    match tryGetNextLink site response with
    | None -> items
    | Some(nextLink) -> getAllLinkedItems<'T> site nextLink queryParams items


let GetAll<'T> (site: CanvasSite) endpoint queryParams =
    let link = endpoint |> site.CreateUrl 
    getAllLinkedItems<'T> site link queryParams []
