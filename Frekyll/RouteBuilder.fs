module Frekyll.Route

open System.IO

type Route = Route of (string -> string)

type RouteBuilder() =
    member this.Bind((Route m): Route, f) =
       this.Return (f << m)
       
    member this.Return(x) = Route x
    
let route = RouteBuilder()

let replaceExtension ext (filePath: string) =
//    let filePath = Path.GetFileNameWithoutExtension(filePath)
    Path.ChangeExtension(filePath, ext)