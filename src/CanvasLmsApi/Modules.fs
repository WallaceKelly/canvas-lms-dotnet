namespace CanvasLmsApi

open System
open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

// From https://canvas.instructure.com/doc/api/modules.html

[<CLIMutable>]
type Module =
    { Id: Int64
      Position: int
      Name: string }

module Modules =

    let Endpoint = "/api/v1/courses/:course_id/modules"

    let Get(site, courseId: Int64) =
        let endpoint = Endpoint.Replace(":course_id", courseId.ToString())
        HttpUtils.GetAll<Module> site endpoint []