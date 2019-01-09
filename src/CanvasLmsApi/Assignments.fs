namespace CanvasLmsApi

open System

// From https://canvas.instructure.com/doc/api/assignments.html

type AssignmentSubmissionType =
     | NoSubmission
     | DiscussionTopic
     | OnlineQuiz
     | OnPaper
     | ExternalTool
     | OnlineTextEntry
     | OnlineUrl
     | OnlineUpload
     | MediaRecording
     static member FromString (s: string) =
        let submissionTypeStrings = 
            [ "none", NoSubmission 
              "discussion_topic", DiscussionTopic 
              "online_quiz", OnlineQuiz
              "on_paper", OnPaper
              "external_tool", ExternalTool
              "online_text_entry", OnlineTextEntry
              "online_url", OnlineUrl
              "online_upload", OnlineUpload
              "media_recording", MediaRecording
            ] |> dict
        match submissionTypeStrings.ContainsKey(s) with
        | true -> submissionTypeStrings.[s]
        | false -> NoSubmission

[<CLIMutable>]
type Assignment =
    { Id: Int64
      Name: string
      HtmlUrl: string
      SubmissionTypes: string[]
      Published: bool
      QuizId: Nullable<Int64> }
    member x.AssignmentSubmissionTypes = x.SubmissionTypes |> Array.map(AssignmentSubmissionType.FromString)

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