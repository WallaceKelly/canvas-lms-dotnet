namespace CanvasLmsApi

open System

[<CLIMutable>]
type CanvasSite =
    { BaseUrl: string 
      AccessToken: string }
      
    member x.AppendAccessToken(url: string) =
        let delimiter =
          match url.Contains("?") with
          | false -> '?'
          | true -> '&'
        sprintf "%s%caccess_token=%s" url delimiter x.AccessToken
    
    member x.CreateUrl(endpoint: string) =
        sprintf "%s%s" x.BaseUrl endpoint
        |> x.AppendAccessToken
        

