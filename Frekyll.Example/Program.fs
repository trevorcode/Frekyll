open System.IO
open Frekyll
open Frekyll.Rule

let posts: RuleBuilder =
    {
        Path = PathParser.parsePath (Path.Combine (PathParser.rootPath, "input/posts"))
        RouteRewrite = Route.replaceExtension ".html"
        Compiler = Some Core.convertMdToHtml
        TemplateUrl = Some "templates/index.html"
    }

let css: RuleBuilder =
    {
        Path = PathParser.parsePath (Path.Combine (PathParser.rootPath, "input/css"))
        RouteRewrite = id
        Compiler = None
        TemplateUrl = None
    }
    
RuleBuilder.build posts
RuleBuilder.build css