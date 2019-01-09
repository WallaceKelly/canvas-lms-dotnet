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
            "/api/v1/courses/:course_id/pages",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Page>

    let Get(site, accessToken, courseId: Int64, pageUrl: string) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/pages/:url",
            [ "course_id", courseId :> obj
              "url", pageUrl :> obj ])
        |> HttpUtils.GetSingle<Page>