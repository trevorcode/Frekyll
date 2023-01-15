open System.IO
open Frekyll


printfn "%A" PathParser.rootPath


let template = File.ReadAllText(PathParser.rootPath + "templates/index.html")
let text = File.ReadAllText(PathParser.rootPath + "input/posts/2015-08-23-example.markdown")

let ctx = Core.convertMdToHtml text
let res = TemplateParser.parse template ctx

printfn "%O" res