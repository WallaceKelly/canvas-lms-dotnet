namespace CanvasLmsApi

open System

// From https://canvas.instructure.com/doc/api/assignments.html

[<CLIMutable>]
type Assignment =
    { Id: Int64
      Name: string }

module Assignments =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/assignments",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Assignment>

    let Get(site, accessToken, courseId: Int64, assignmentId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/assignments/:assignment_id",
            [ "course_id", courseId
              "assignment_id", assignmentId ])
        |> HttpUtils.GetSingle<Assignment>