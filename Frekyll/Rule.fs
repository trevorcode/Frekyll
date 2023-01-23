namespace Frekyll.Rule

open System.IO
open Frekyll
open Frekyll.PathParser

type RuleBuilder =
    { Path: FileType
      TemplateUrl: (string) option
      Compiler: (string -> Context) option
      RouteRewrite: string -> string }

module RuleBuilder =
    open FsToolkit.ErrorHandling
    let removePathFromOther (p1: string) (p2: string) =
        p2.Substring(p1.Length)

    let putValuesIntoTemplate filePath url compiler =
        let template = File.ReadAllText(PathParser.rootPath + url)
        let text = File.ReadAllText(filePath)
        
        let ctx = compiler text

        let res = TemplateParser.parse template ctx
        res             
        
    let getValuesFromFile filePath =
        File.ReadAllText(filePath)

        
    let runSingleFile (builder: RuleBuilder) filePath =
        let urlAndCompiler = Option.zip (builder.TemplateUrl) builder.Compiler
        let res = 
            urlAndCompiler
            |> Option.either
                (fun (url, compiler) -> 
                    putValuesIntoTemplate filePath url compiler)
                (fun () -> getValuesFromFile filePath)
        
        let cleanedPath = removePathFromOther rootPath filePath
        let targetPath = Path.Join(rootPath, "site", cleanedPath)
        let newFilePath = builder.RouteRewrite targetPath

        let fileName = Path.GetFileName(newFilePath)
        let directories = newFilePath.Replace(fileName, "")
        Directory.CreateDirectory(directories) |> ignore
        
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