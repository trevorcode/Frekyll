namespace Frekyll.Rule

open System.IO
open Frekyll
open Frekyll.PathParser

type RuleBuilder =
    { Path: FileType
      Template: string
      Transform: string -> Context
      Route: string -> string }

module RuleBuilder =
    let removePathFromOther (p1: string) (p2: string) =
        p2.Substring(p1.Length)
        
    let runSingleFile builder filePath=
        let template = File.ReadAllText(PathParser.rootPath + builder.Template)
        let text = File.ReadAllText(filePath)
        
        let ctx = builder.Transform text
        
        let res = TemplateParser.parse template ctx
        
        let cleanedPath = removePathFromOther rootPath filePath
        let targetPath = Path.Join(rootPath, "site", cleanedPath)
        let newFilePath = builder.Route targetPath
        
        //Todo: make sure directories exist before writing to files
//        let directories = Directory.Get(newFilePath)
        
        
        File.WriteAllText(newFilePath, res)
    
    let build builder =
        match builder.Path with
        | SingleFile filePath ->
            runSingleFile builder filePath
            
        | Directory d -> 
            let files = Directory.EnumerateFiles(d)
            
            files
            |> Seq.map (fun filePath -> runSingleFile builder filePath)
            |> Seq.toList
            |> ignore