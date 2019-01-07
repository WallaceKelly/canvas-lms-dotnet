namespace CanvasLmsApi

open System
open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

// From https://canvas.instructure.com/doc/api/courses.html

[<CLIMutable>]
type Term =
    { Id: Int64
      Name: string }

[<CLIMutable>]
type Course =
    { Id: Int64
      EnrollmentTermId: Int64
      Term: Term
      Name: string }

module Courses =

    let Endpoint = "/api/v1/courses"

    let Get site = HttpUtils.GetAll<Course> site Endpoint [("include[]", "term")]
