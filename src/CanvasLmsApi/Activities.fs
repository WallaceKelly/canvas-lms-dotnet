namespace CanvasLmsApi

open System

// This is not part of the API but may be useful

type Activity =
     | Page of string * string // url, title
     | Assignment of Int64 * string // id, name
     | Discussion of Int64 * string // id, title
     | Quiz of Int64 * string // id, title

module Activities =

    let GetAll(site, accessToken, courseId: Int64) =

        let pages = Pages.GetAll(site, accessToken, courseId) |> Seq.map(fun p -> Activity.Page(p.Url, p.Title))
        let asgns = Assignments.GetAll(site, accessToken, courseId) |> Seq.map(fun a -> Activity.Assignment(a.Id, a.Name))
        let discs = Discussions.GetAll(site, accessToken, courseId) |> Seq.map(fun d -> Activity.Discussion(d.Id, d.Title))
        let quizs = Quizzes.GetAll(site, accessToken, courseId) |> Seq.map(fun q -> Activity.Quiz(q.Id, q.Title))

        pages
        |> Seq.append asgns
        |> Seq.append discs
        |> Seq.append quizs
        |> Seq.cache

    let Get<'T>(site, accessToken, courseId: Int64, activity: Activity) =
        match activity with
        | Page(url, _title) ->
            if typeof<'T> <> typeof<Page> then failwith "Activity is not a Page."
            else Pages.Get(site, accessToken, courseId, url) |> Option.map(fun p -> p :> obj)
        | Assignment(assignmentId, _name) ->
            if typeof<'T> <> typeof<Assignment> then failwith "Activity is not an Assignment."
            else Assignments.Get(site, accessToken, courseId, assignmentId) |> Option.map(fun p -> p :> obj)
        | Discussion(discussionId, _title) ->
            if typeof<'T> <> typeof<Discussion> then failwith "Activity is not a Discussion."
            else Discussions.Get(site, accessToken, courseId, discussionId) |> Option.map(fun d -> d :> obj)
        | Quiz(quizId, _title) ->
            if typeof<'T> <> typeof<Quiz> then failwith "Activity is not a Quiz."
            else Quizzes.Get(site, accessToken, courseId, quizId) |> Option.map(fun q -> q :> obj)
        |> Option.map(fun o -> o :?> 'T)