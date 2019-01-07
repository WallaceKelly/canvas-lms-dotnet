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

    let site = 
        { CanvasSite.BaseUrl = readCommandLine "Enter the Canvas site" 0 argv
          AccessToken = readCommandLine "Enter your access token" 1 argv }

    let courses = Courses.Get(site)

    for c in courses do
        printfn "%d\t%s\t%s" c.Id c.Term.Name c.Name

    let courseId = getInput "Enter a course number" |> Convert.ToInt64

    let modules = Modules.Get(site, courseId)

    for m in modules do
        printfn "%d\t%s" m.Id m.Name

    0 // return an integer exit code
