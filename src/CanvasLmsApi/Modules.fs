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

type ModuleItemContentId =
     | Page of string // page_url
     | Other of Int64 // content_id

type ModuleItemCompletionRequirement = 
     | NoRequirement
     | MustView
     | MustContribute
     | MustSubmit
     override x.ToString() =
        match x with
        | NoRequirement -> ""
        | MustView -> "must_view"
        | MustContribute -> "must_contribute"
        | MustSubmit -> "must_submit"

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
            "/api/v1/courses/:course_id/modules",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Module>

    let Get(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules",
            [ "course_id", courseId
              "module_id", moduleId ])
        |> HttpUtils.GetAll<Module>

    let Create(site, accessToken, courseId: Int64, name: string) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules",
            [ "course_id", courseId :> obj
              "module[name]", name :> obj ]
        )
        |> HttpUtils.Post<Module>

    let GetItems(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id/items",
            [ "course_id", courseId
              "module_id", moduleId ])
        |> HttpUtils.GetAll<ModuleItem> 

    let CreateItem(site, accessToken, courseId: Int64, moduleId: Int64,
                    itemType: ModuleItemType, itemContentId: ModuleItemContentId, requirement: ModuleItemCompletionRequirement) =
        let contentIdParameter =
            match itemContentId with
            | Page(s) -> "module_item[page_url]", s :> obj
            | Other(id) -> "module_item[content_id]", id :> obj
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id/items",
            [ "course_id", courseId :> obj
              "module_id", moduleId :> obj
              "module_item[type]", itemType.ToString() :> obj
              contentIdParameter
              "module_item[completion_requirement][type]", requirement.ToString() :> obj] )
        |> HttpUtils.Post<ModuleItem> 

    let Delete(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id",
            [ "course_id", courseId
              "module_id", moduleId ])
        |> HttpUtils.Delete<ModuleItem>