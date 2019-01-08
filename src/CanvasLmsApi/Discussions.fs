namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/discussion_topics.html

[<CLIMutable>]
type Discussion =
    { Id: Int64
      Title: string }

module Discussions =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/discussion_topics",
            [ "courseId", courseId ])
        |> HttpUtils.GetAll<Discussion>

    let Get(site, accessToken, courseId: Int64, discussionId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:courseId/discussion_topics/:discussionId",
            [ "courseId", courseId; "discussionId", discussionId ])
        |> HttpUtils.GetSingle<Discussion>