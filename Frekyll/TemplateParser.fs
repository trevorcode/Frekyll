module Frekyll.TemplateParser

open FParsec

type TemplateExpr = Identity of string

type TemplateElement =
    | Chunk of string
    | Expr of TemplateExpr
    | If of TemplateExpr * TemplateElement list * TemplateElement list option
    | For of TemplateExpr * TemplateElement list

let ws = spaces
let trimOpen: Parser<_, unit> = skipString "{{"
let trimClose: Parser<_, unit> = skipString "}}"
let templateElem, templateElemRef = createParserForwardedToRef ()

let expr =
    trimOpen >>? ws >>? skipString "id" >>? ws
    >>. manyChars (letter <|> digit)
    .>> ws
    .>> trimClose
    |>> Identity

let endExpr =
    trimOpen >>? ws >>? skipString "end"
    .>> ws
    .>> trimClose

let conditional =
    trimOpen
    >>? ws
    >>? skipString "if"
    >>? ws
    >>? manyChars (letter <|> digit)
    .>> ws
    .>> trimClose
    .>>. templateElem
    .>> endExpr
    |>> fun (ifId, templateNodes) -> If(Identity ifId, templateNodes, None)


let forExpr =
    trimOpen
    >>? ws
    >>? skipString "for"
    >>? ws
    >>? manyChars (letter <|> digit)
    .>> ws
    .>> trimClose
    .>>. templateElem
    .>> endExpr
    |>> fun (ifId, templateNodes) -> For(Identity ifId, templateNodes)

let identity = expr |>> Expr

let rawHtml = many1Chars (noneOf "{{") |>> Chunk

templateElemRef
:= many
   <| choice [ forExpr
               conditional
               identity
               rawHtml ]

let run text =
    match run templateElem text with
    | Success (res, _, _) -> res
    | Failure (err, _, _) -> failwith err

let execute parsedTemplate (context: Context) =
    let rec evaluate (acc: string) (subContext: Context) expr =
        match expr with
        | Chunk c -> acc + c
        | Expr (Identity i) ->
            printfn "Identity: %s" i
            acc
            + (subContext
               |> Context.getProperty i
               |> Context.value)
            
        | If (Identity i, subTemplate, _) ->
            if (Context.propertyExists i subContext) then
                let res =
                    subTemplate
                    |> List.map (evaluate "" subContext)
                    |> List.reduce (+)

                res
            else
                ""
        | For (Identity i, subTemplate) ->
            let res =
                subTemplate
                |> List.map (evaluate "" subContext)
                |> List.reduce (+)

            i + res

    let res =
        parsedTemplate
        |> List.map (evaluate "" context)
        |> List.reduce (+)

    res

let parse = run >> execute
