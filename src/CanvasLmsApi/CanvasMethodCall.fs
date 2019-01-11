namespace CanvasLmsApi

open System

type CanvasMethodCall =
    { BaseUrl: string
      AccessToken: string
      Endpoint: string
      Parameters: list<string * obj> }

    static member Create(baseUrl, accessToken, endpoint) =
        { CanvasMethodCall.BaseUrl = baseUrl
          AccessToken = accessToken 
          Endpoint = endpoint
          Parameters = [] }

    static member Create(baseUrl, accessToken, endpoint, parameters: list<string * obj>) =
        { CanvasMethodCall.BaseUrl = baseUrl
          AccessToken = accessToken 
          Endpoint = endpoint
          Parameters = parameters }

    static member Create(baseUrl, accessToken, endpoint, parameters: list<string * string>) =
        CanvasMethodCall.Create(baseUrl, accessToken, endpoint,
            parameters |> List.map(fun (name, value) -> name, value :> obj ))

    static member Create(baseUrl, accessToken, endpoint, parameters: list<string * Int64>) =
        CanvasMethodCall.Create(baseUrl, accessToken, endpoint,
            parameters |> List.map(fun (name, value) -> name, value :> obj ))

    member x.GetUrlString() =

        let replaceEndpointParameter (endpoint: string) (name: string, value: obj) =
            let placeholder = sprintf ":%s" name
            match endpoint.Contains(placeholder) with
            | false -> endpoint
            | true -> endpoint.Replace(placeholder, value.ToString())
            
        let endpoint = x.Parameters |> Seq.fold replaceEndpointParameter x.Endpoint

        sprintf "%s%s?access_token=%s"
            x.BaseUrl
            endpoint
            x.AccessToken

    member x.GetQueryParameters() =
        let isUrlParameter (name: string, _value: obj) =
            let placeholder = sprintf ":%s" name
            x.Endpoint.Contains(placeholder)
        x.Parameters
        |> List.where(isUrlParameter >> not)
        |> List.map(fun (name, value) -> name, if value = null then String.Empty else value.ToString())

        
        
