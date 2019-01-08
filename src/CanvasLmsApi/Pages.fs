namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/pages.html

[<CLIMutable>]
type Page =
    { Url: string
      Title: string }

module Pages =

    let Get(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/pages",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Module>