namespace CanvasLmsApi

open System

// From https://canvas.instructure.com/doc/api/courses.html

[<CLIMutable>]
type Term =
    { Id: Int64
      Name: string }

[<CLIMutable>]
type Course =
    { Id: Int64
      EnrollmentTermId: Int64
      Term: Term
      Name: string }

module Courses =

    let GetAll(site, accessToken) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses",
            [ "include[]", "term" ])
        |> HttpUtils.GetAll<Course>

    let Get(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId",
            [ "include[]", "term" :> obj
              "courseId", courseId :> obj])
        |> HttpUtils.GetSingle<Course>
