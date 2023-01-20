open System.IO
open Frekyll
open Frekyll.Rule

let posts: RuleBuilder =
    {
        Path = PathParser.parsePath (Path.Combine (PathParser.rootPath, "input/posts"))
        Route = Route.replaceExtension ".html"
        Transform = Core.convertMdToHtml
        Template = "templates/index.html"
        
    }
    
RuleBuilder.build posts

//let template = File.ReadAllText(PathParser.rootPath + "templates/index.html")
//let text = File.ReadAllText(PathParser.rootPath + "input/posts/2015-08-23-example.markdown")
//
//let ctx = Core.convertMdToHtml text
//let res2 = TemplateParser.run' template
//printfn "%O" res2
//let res = TemplateParser.parse template ctx
//
//printfn "%O" res