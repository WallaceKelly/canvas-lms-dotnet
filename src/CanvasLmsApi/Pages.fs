namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/pages.html

[<CLIMutable>]
type Page =
    { Url: string
      Title: string }

module Pages =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/pages",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Page>

    let Get(site, accessToken, courseId: Int64, pageId: string) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/pages",
            [ "courseId", courseId :> obj; "pageId", pageId :> obj ])
        |> HttpUtils.GetSingle<Page>