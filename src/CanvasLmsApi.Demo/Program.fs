// Learn more about F# at http://fsharp.org

open System
open CanvasLmsApi

let getInput desc =
    printf "%s: " desc
    Console.ReadLine()

let readCommandLine desc argi (argv: string[]) =
    if argv.Length <= argi then getInput desc
    else argv.[argi]

[<EntryPoint>]
let main argv =

    let site = readCommandLine "Enter the Canvas site" 0 argv
    let accessToken = readCommandLine "Enter your access token" 1 argv

    let courses = Courses.GetAll(site, accessToken)
    for c in courses do
        printfn "%d\t%s\t%s" c.Id c.Term.Name c.Name

    let courseId = getInput "Enter a course number" |> Convert.ToInt64

    //let modules = Modules.GetAll(site, accessToken, courseId)
    //for m in modules do
    //    printfn "%d\t%s" m.Id m.Name

    //let moduleId = getInput "Enter a module number" |> Convert.ToInt64
    //let moduleItems = Modules.GetItems(site, accessToken, courseId, moduleId)
    //for i in moduleItems do
    //    printfn "%d\t%s (%A)" i.Id i.Title i.ModuleItemType

    let activities = Activities.GetAll(site, accessToken, courseId)
    for a in activities do
        printfn "%A" a

    let title = getInput "Enter an existing activity title"
    let activity = activities |> Seq.tryFind(fun a -> a.Title = title)
    match activity with
    | None -> failwith "Could not find matching activity"
    | Some(a) ->

        let newModuleName = getInput "Enter a new module name" 
        let newModule = Activities.CreateModule(site, accessToken, courseId, newModuleName, [ a ])
        let newModuleItems = Modules.GetItems(site, accessToken, courseId, newModule.Id)
        for i in newModuleItems do
            printfn "%d\t%s (%A)" i.Id i.Title i.ModuleItemType


    
    0 // return an integer exit code
