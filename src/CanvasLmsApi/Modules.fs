namespace CanvasLmsApi

open System

// From https://canvas.instructure.com/doc/api/modules.html

(*  // Module Item
    {
      // the unique identifier for the module item
      "id": 768,
      // the id of the Module this item appears in
      "module_id": 123,
      // the position of this item in the module (1-based)
      "position": 1,
      // the title of this item
      "title": "Square Roots: Irrational numbers or boxy vegetables?",
      // 0-based indent level; module items may be indented to show a hierarchy
      "indent": 0,
      // the type of object referred to one of 'File', 'Page', 'Discussion',
      // 'Assignment', 'Quiz', 'SubHeader', 'ExternalUrl', 'ExternalTool'
      "type": "Assignment",
      // the id of the object referred to applies to 'File', 'Discussion',
      // 'Assignment', 'Quiz', 'ExternalTool' types
      "content_id": 1337,
      // link to the item in Canvas
      "html_url": "https://canvas.example.edu/courses/222/modules/items/768",
      // (Optional) link to the Canvas API object, if applicable
      "url": "https://canvas.example.edu/api/v1/courses/222/assignments/987",
      // (only for 'Page' type) unique locator for the linked wiki page
      "page_url": "my-page-title",
      // (only for 'ExternalUrl' and 'ExternalTool' types) external url that the item
      // points to
      "external_url": "https://www.example.com/externalurl",
      // (only for 'ExternalTool' type) whether the external tool opens in a new tab
      "new_tab": false,
      // Completion requirement for this module item
      "completion_requirement": {"type":"min_score","min_score":10,"completed":true},
      // (Present only if requested through include[]=content_details) If applicable,
      // returns additional details specific to the associated object
      "content_details": {"points_possible":20,"due_at":"2012-12-31T06:00:00-06:00","unlock_at":"2012-12-31T06:00:00-06:00","lock_at":"2012-12-31T06:00:00-06:00"},
      // (Optional) Whether this module item is published. This field is present only
      // if the caller has permission to view unpublished items.
      "published": true
    }
*)

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

    let Get(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site,
            accessToken,
            "/api/v1/courses/:courseId/modules",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Module>

    let GetItems(site, accessToken, courseId: Int64, moduleId: Int64) =
        CanvasMethodCall.Create(
            site,
            accessToken,
            "/api/v1/courses/:courseId/modules/:moduleId/items",
            [ "courseId", courseId
              "moduleId", moduleId ])
        |> HttpUtils.GetAll<ModuleItem> 

    //let CreateModuleItem(site, courseId: Int64, newModule: ModuleItem) =
    //    let endpoint = sprintf "/api/v1/courses/%d/modules/%d/items" courseId newModule.ModuleId
    //    HttpUtils.Post()