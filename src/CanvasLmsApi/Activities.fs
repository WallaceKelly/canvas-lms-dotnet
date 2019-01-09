namespace CanvasLmsApi

open System

// This is not part of the API but may be useful

type Activity =
     | Page of string * string // url, title
     | Assignment of Int64 * string // id, name
     | Discussion of Int64 * string // id, title
     | Quiz of Int64 * string // id, title
     static member ToModuleItemType a =
        match a with
        | Assignment(_) -> ModuleItemType.Assignment
        | Discussion(_) -> ModuleItemType.Discussion
        | Quiz(_) -> ModuleItemType.Quiz
        | Page(_) -> ModuleItemType.Page
     static member ToModuleItemContentId a =
        match a with
        | Assignment(id, _)
        | Discussion(id, _)
        | Quiz(id, _) -> ModuleItemContentId.Other(id)
        | Page(pageUrl, _) -> ModuleItemContentId.Page(pageUrl)
     member x.Title = 
        match x with
        | Page(_, title) -> title
        | Assignment(_, name) -> name
        | Discussion(_, title) -> title
        | Quiz(_, title) -> title
     member x.ModuleItemContentId = x |> Activity.ToModuleItemContentId
     member x.ModuleItemType = x |> Activity.ToModuleItemType

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

    let CreateModule(site, accessToken, courseId: Int64, moduleName: string, activities: Activity seq) =
        let newModule = Modules.Create(site, accessToken, courseId, moduleName)
        let getRequirement (a: Activity) =
            match a with
            | Page(_) -> ModuleItemCompletionRequirement.MustView
            | Assignment(_) -> ModuleItemCompletionRequirement.MustSubmit
            | Discussion(_) -> ModuleItemCompletionRequirement.MustContribute
            | Quiz(_) -> ModuleItemCompletionRequirement.MustSubmit
        let createModuleItem(itemType, contentId, requirement) =
            Modules.CreateItem(site, accessToken, courseId, newModule.Id, itemType, contentId, requirement)
            |> ignore
        activities
        |> Seq.map(fun a -> a.ModuleItemType, a.ModuleItemContentId, a |> getRequirement)
        |> Seq.iter(createModuleItem)
        newModule
