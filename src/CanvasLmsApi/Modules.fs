namespace CanvasLmsApi

open System

// From https://canvas.instructure.com/doc/api/modules.html

type ModuleItemType =
     | Unknown
     | File
     | Page
     | Discussion
     | Assignment
     | Quiz
     | SubHeader
     | ExternalUrl
     | ExternalTool
    with
        override this.ToString() = FSharpUtils.toString this
        static member fromString s = FSharpUtils.fromString<ModuleItemType> s


[<CLIMutable>]
type ModuleItem =
    { Id: Int64
      ModuleId: Int64
      Position: int
      Title: string
      Type: string
      ContentId: Int64 }
    member x.ModuleItemType =
        match ModuleItemType.fromString(x.Type) with
        | Some(v) -> v
        | None -> Unknown
        

[<CLIMutable>]
type Module =
    { Id: Int64
      Position: int
      Name: string }

module Modules =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/modules",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Module>

    let GetItems(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/modules/:moduleId/items",
            [ "courseId", courseId; "moduleId", moduleId ])
        |> HttpUtils.GetAll<ModuleItem> 

    //let CreateModuleItem(site, courseId: Int64, newModule: ModuleItem) =
    //    let endpoint = sprintf "/api/v1/courses/%d/modules/%d/items" courseId newModule.ModuleId
    //    HttpUtils.Post()