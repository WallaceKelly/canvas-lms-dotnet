namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/pages.html

[<CLIMutable>]
type Page =
    { Url: string // unique identifier
      Title: string
      HtmlUrl: string
      Published: bool }

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

    let Edit(site, accessToken, courseId: Int64, pageUrl: string, published: bool) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/pages/:page_url",
            [ "course_id", courseId :> obj
              "page_url", pageUrl :> obj
              "wiki_page[published]", published :> obj] )
        |> HttpUtils.Put<Page>
        