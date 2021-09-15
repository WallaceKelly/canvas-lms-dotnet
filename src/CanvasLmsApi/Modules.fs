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
     override x.ToString() = 
        match x with
        | Page(page_url) -> page_url
        | Other(content_id) -> content_id.ToString()

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
      PageUrl: string // for pages
      ContentId: Int64 // for assignments, quizzes, etc.
    }
    member x.ModuleItemType =
        match ModuleItemType.fromString(x.Type) with
        | Some(v) -> v
        | None -> Unknown
    member x.IdString =
        match x.ModuleItemType with
        | ModuleItemType.Page -> x.PageUrl
        | _ -> x.ContentId.ToString()
        

[<CLIMutable>]
type Module =
    { Id: Int64
      Position: int
      Name: string
      PrerequisiteModuleIds: Int64[]
      RequireSequentialProgress: bool
      Published: bool }

module Modules =

    //let createListString (items: 'a seq) =
    //    items
    //    |> Seq.map(fun i -> i.ToString())
    //    |> Seq.toArray
    //    |> fun ids -> sprintf "[%s]" (String.Join(",", ids))

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Module>

    let Get(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id",
            [ "course_id", courseId
              "module_id", moduleId ])
        |> HttpUtils.GetSingle<Module>

    let Create(site, accessToken, courseId: Int64, name: string, requireSequentialProgress: bool, prerequisiteModuleId: Int64[]) =
        let prerequisiteModuleIdString = if prerequisiteModuleId.Length > 0 then prerequisiteModuleId.[0].ToString() else null
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules",
            [ "course_id", courseId :> obj
              "module[name]", name :> obj
              "module[require_sequential_progress]", requireSequentialProgress :> obj
              "module[prerequisite_module_ids][]", prerequisiteModuleIdString :> obj
            ])
        |> HttpUtils.Post<Module>

    let Edit(site, accessToken, courseId: Int64, moduleId: Int64, published: bool, requireSequentialProgress: bool, prerequisiteModuleId: Int64[]) =
        let prerequisiteModuleIdString = if prerequisiteModuleId <> null && prerequisiteModuleId.Length > 0 then prerequisiteModuleId.[0].ToString() else null
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id",
            [ "course_id", courseId :> obj
              "module_id", moduleId :> obj
              "module[published]", published :> obj
              "module[require_sequential_progress]", requireSequentialProgress :> obj
              "module[prerequisite_module_ids][]", prerequisiteModuleIdString :> obj
            ])
        |> HttpUtils.Put<Module>

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
              if requirement <> NoRequirement then
                "module_item[completion_requirement][type]", requirement.ToString() :> obj] )
        |> HttpUtils.Post<ModuleItem> 

    let Delete(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id",
            [ "course_id", courseId
              "module_id", moduleId ])
        |> HttpUtils.Delete<Module>


    let DeleteItem(site, accessToken, courseId: Int64, moduleId: Int64, moduleItemId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/modules/:module_id/items/:module_item_id",
            [ "course_id", courseId
              "module_id", moduleId
              "module_item_id", moduleItemId ])
        |> HttpUtils.Delete<ModuleItem>

        
    let DeleteItems(site, accessToken, courseId, moduleId) =
        seq {
            for mi in GetItems(site, accessToken, courseId, moduleId) do
                yield DeleteItem(site, accessToken, courseId, moduleId, mi.Id)
        }
        |> Seq.toArray // force it to run
        |> Array.toSeq

