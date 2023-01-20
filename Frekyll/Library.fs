namespace Frekyll

open System
open System.Collections.Generic
open System.IO
open Frekyll.PathParser
open Markdig
open Markdig.Syntax
open Markdig.Extensions.Yaml

type Field =
    | Text of string
    | List of List<string>

and Context = Context of IDictionary<string, Field>

module Context =
    let getProperty (propertyName: string) (Context ctxList: Context) : Field =
        ctxList.Item propertyName

    let propertyExists (name) (Context ctxList: Context) = ctxList.ContainsKey(name)

    let value (ctxType: Field) : string =
        match ctxType with
        | Text t -> t
        | List _ -> failwith "Tried to read property of context type"

module Core =
    let rec convertMdToHtml (mdString: string) : Context =
        let pipeline =
            MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build()

        let doc = Markdown.Parse(mdString, pipeline)
        let html = doc.ToHtml(pipeline)

        let yamlBlock =
            doc.Descendants<YamlFrontMatterBlock>()
            |> Seq.tryExactlyOne

        let res =
            yamlBlock
            |> Option.map (fun x ->
                x.Lines.Lines
                |> Array.sortByDescending (fun x -> x.Line)
                |> Array.map (fun x -> $"{x}\n")
                |> Array.map (fun x -> x.Replace("---", ""))
                |> Array.filter (fun x -> String.IsNullOrWhiteSpace(x) |> not)
                |> Array.choose (fun x ->
                    let values = x.Split(':')
                    let key = Array.tryHead values

                    let value =
                        values
                        |> Array.skip 1
                        |> Array.tryHead
                        |> Option.defaultValue ""

                    key |> Option.map (fun key -> (key, Text value))))
            |> Option.get
            |> Array.toList

        Context(dict <| ("body", Text html) :: res)


