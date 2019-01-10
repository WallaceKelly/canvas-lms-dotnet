namespace CanvasLmsApi

open System

// This is not part of the API but may be useful

type Activity =
     | Page of CanvasLmsApi.Page
     | Assignment of CanvasLmsApi.Assignment
     | Discussion of CanvasLmsApi.Discussion
     | Quiz of CanvasLmsApi.Quiz

     static member ToModuleItemType a =
        match a with
        | Assignment(_) -> ModuleItemType.Assignment
        | Discussion(_) -> ModuleItemType.Discussion
        | Quiz(_) -> ModuleItemType.Quiz
        | Page(_) -> ModuleItemType.Page
     static member ToModuleItemContentId a =
        match a with
        | Assignment(a) -> ModuleItemContentId.Other(a.Id)
        | Discussion(d) -> ModuleItemContentId.Other(d.Id)
        | Quiz(q) -> ModuleItemContentId.Other(q.Id)
        | Page(p) -> ModuleItemContentId.Page(p.Url)
     static member GetHtmlUrl a =
        match a with
        | Assignment(a) -> a.HtmlUrl
        | Discussion(d) -> d.HtmlUrl
        | Quiz(q) -> q.HtmlUrl
        | Page(p) -> p.HtmlUrl
     static member GetTitle a =
        match a with
        | Assignment(a) -> a.Name
        | Discussion(d) -> d.Title
        | Quiz(q) -> q.Title
        | Page(p) -> p.Title
     static member ToActivityType a =
        match a with
        | Assignment(a) -> "Assignment"
        | Discussion(d) -> "Discussion"
        | Quiz(q) -> "Quiz"
        | Page(p) -> "Page"
     static member ToIdString a =
        match a with
        | Assignment(a) -> a.Id.ToString()
        | Discussion(d) -> d.Id.ToString()
        | Quiz(q) -> q.Id.ToString()
        | Page(p) -> p.Url

     member x.ActivityType = x |> Activity.ToActivityType
     member x.IdString = x |> Activity.ToIdString
     member x.Title = x |> Activity.GetTitle
     member x.HtmlUrl = x |> Activity.GetHtmlUrl
     member x.ModuleItemContentId = x |> Activity.ToModuleItemContentId
     member x.ModuleItemType = x |> Activity.ToModuleItemType

     member x.IsPublished = 
        match x with
        | Assignment(a) -> a.Published
        | Discussion(d) -> d.Published
        | Quiz(q) -> q.Published
        | Page(p) -> p.Published

module Activities =

    let GetAll(site, accessToken, courseId: Int64) =

        let pages = Pages.GetAll(site, accessToken, courseId) |> Seq.map(fun p -> Activity.Page(p))
        let discs = Discussions.GetAll(site, accessToken, courseId) |> Seq.map(fun d -> Activity.Discussion(d))
        let quizs = Quizzes.GetAll(site, accessToken, courseId) |> Seq.map(fun q -> Activity.Quiz(q))
        let asgns =
            let isNotQuizOrDiscussion (a: Assignment) =
                a.AssignmentSubmissionTypes
                |> Array.exists(fun t -> t = DiscussionTopic || t = OnlineQuiz)
                |> not
            Assignments.GetAll(site, accessToken, courseId)
            |> Seq.where(isNotQuizOrDiscussion)
            |> Seq.map(fun a -> Activity.Assignment(a))

        pages
        |> Seq.append asgns
        |> Seq.append discs
        |> Seq.append quizs
        |> Seq.cache

    let Get<'T>(site, accessToken, courseId: Int64, activity: Activity) =
        match activity with
        | Page(p) ->
            if typeof<'T> <> typeof<Page> then failwith "Activity is not a Page."
            else p :> obj
        | Assignment(a) ->
            if typeof<'T> <> typeof<Assignment> then failwith "Activity is not an Assignment."
            else a :> obj
        | Discussion(d) ->
            if typeof<'T> <> typeof<Discussion> then failwith "Activity is not a Discussion."
            else d :> obj
        | Quiz(q) ->
            if typeof<'T> <> typeof<Quiz> then failwith "Activity is not a Quiz."
            else q :> obj
        |> fun o -> o :?> 'T

    let CreateModule(site, accessToken, courseId: Int64, moduleName: string, activities: Activity seq, isPublished: bool) =
        let newModule = Modules.Create(site, accessToken, courseId, moduleName, isPublished)
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

    let Edit(site, accessToken, courseId: Int64, activity: Activity, isPublished: bool) =
        match activity with
        | Page(p) -> Pages.Edit(site, accessToken, courseId, p.Url, isPublished) |> Activity.Page
        | Assignment(a) -> Assignments.Edit(site, accessToken, courseId, a.Id, isPublished) |> Activity.Assignment
        | Discussion(d) -> Discussions.Edit(site, accessToken, courseId, d.Id, isPublished) |> Activity.Discussion
        | Quiz(q) -> Quizzes.Edit(site, accessToken, courseId, q.Id, isPublished) |> Activity.Quiz