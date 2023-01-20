namespace Frekyll.Rule

open System.IO
open Frekyll
open Frekyll.PathParser

type RuleBuilder =
    { Path: FileType
      Template: string
      Transform: string -> Context
      Route: string }

module RuleBuilder =
    let runSingleFile builder filePath=
        let template = File.ReadAllText(PathParser.rootPath + builder.Template)
        let text = File.ReadAllText(filePath)
        let fileName = Path.GetFileName(filePath)

        let ctx = builder.Transform text
        
        let res = TemplateParser.parse template ctx
        
        let outputPath = rootPath + builder.Route + fileName
        
        File.WriteAllText(outputPath, res)
    
    let build builder =
        match builder.Path with
        | SingleFile f ->
            runSingleFile builder f
            
        | Directory d -> 
            let files = Directory.EnumerateFiles(d)
            
            files
            |> Seq.map (fun f -> runSingleFile builder f)
            |> Seq.toList
            |> ignore