namespace CanvasLmsApi

open System

// from https://canvas.instructure.com/doc/api/discussion_topics.html

[<CLIMutable>]
type Discussion =
    { Id: Int64
      Title: string
      HtmlUrl: string
      Published: bool }

module Discussions =

    let GetAll(site, accessToken, courseId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/discussion_topics",
            [ "course_id", courseId ])
        |> HttpUtils.GetAll<Discussion>

    let Get(site, accessToken, courseId: Int64, discussionId: Int64) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/discussion_topics/:discussion_id",
            [ "course_id", courseId
              "discussion_id", discussionId ])
        |> HttpUtils.GetSingle<Discussion>

    let Edit(site, accessToken, courseId: Int64, topicId: Int64, published: bool) =
        CanvasMethodCall.Create(
            site, accessToken,
            "/api/v1/courses/:course_id/discussion_topics/:topic_id",
            [ "course_id", courseId :> obj
              "topic_id", topicId :> obj
              "published", published :> obj] )
        |> HttpUtils.Put<Discussion>
        