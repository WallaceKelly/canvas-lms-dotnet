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
    let token = readCommandLine "Enter your access token" 1 argv

    let courses = Courses.GetAll(site, token)
    for c in courses do
        printfn "%d\t%s\t%s" c.Id c.Term.Name c.Name

    let courseId = getInput "Enter a course number" |> Convert.ToInt64

    //let modules = Modules.GetAll(site, token, courseId)
    //for m in modules do
    //    printfn "%d\t%s" m.Id m.Name

    //let moduleId = getInput "Enter a module number" |> Convert.ToInt64
    //let moduleItems = Modules.GetItems(site, token, courseId, moduleId)
    //for i in moduleItems do
    //    printfn "%d\t%s (%A)" i.Id i.Title i.ModuleItemType

    //let activities = Activities.GetAll(site, token, courseId)
    //for a in activities do
    //    printfn "%A" a

    //let title = getInput "Enter an existing activity title"
    //let activity = activities |> Seq.tryFind(fun a -> a.Title = title)
    //match activity with
    //| None -> failwith "Could not find matching activity"
    //| Some(a) ->

    //    let newModuleName = getInput "Enter a new module name" 
    //    let newModule = Activities.CreateModule(site, token, courseId, newModuleName, [ a ], false, false, Seq.empty)
    //    let newModuleItems = Modules.GetItems(site, token, courseId, newModule.Id)
    //    for i in newModuleItems do
    //        printfn "%d\t%s (%A)" i.Id i.Title i.ModuleItemType
    //    printfn "Press any key to delete the new module items..."
    //    Console.ReadKey(true) |> ignore
    //    Modules.DeleteItems(site, token, courseId, newModule.Id)
    //    |> Seq.iter(fun mi -> printfn "\tDeleted: #%d, %s" mi.Id mi.Title)
    //    printfn "Press any key to delete the new module..."
    //    Console.ReadKey(true) |> ignore
    //    Modules.Delete(site, token, courseId, newModule.Id) |> ignore

    let newModule1 = Modules.Create(site, token, courseId, "My New Module #4", true, [||])
    printfn "Created module #%d" newModule1.Id
    Activities.GetAll(site, token, courseId)
    |> Seq.take(5)
    |> Seq.map(fun a ->
        Modules.CreateItem(site, token, courseId, newModule1.Id, a.ModuleItemType, a.ModuleItemContentId, ModuleItemCompletionRequirement.MustView)
       )
    |> Seq.iter(fun mi -> printfn "Added %s" mi.Title)

    let newModule2 = Modules.Create(site, token, courseId, "My New Module #5", true, [| newModule1.Id |])
    printfn "Created module #%d" newModule2.Id
    
    0 // return an integer exit code
