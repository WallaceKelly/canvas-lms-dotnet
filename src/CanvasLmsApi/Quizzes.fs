namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/quizzes.html

[<CLIMutable>]
type Quiz =
    { Id: Int64
      Title: string
      HtmlUrl: string
      Published: bool }

module Quizzes =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/quizzes",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Quiz>

    let Get(site, accessToken, courseId: Int64, quizId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/quizzes/:quiz_id",
            [ "course_id", courseId
              "quiz_id", quizId ])
        |> HttpUtils.GetSingle<Quiz>