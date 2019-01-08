namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/quizzes.html

[<CLIMutable>]
type Quiz =
    { Id: Int64
      Title: string }

module Quizzes =

    let Get(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/quizzes",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Quiz>