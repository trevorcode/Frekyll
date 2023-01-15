﻿module Frekyll.PathParser

open System
open System.IO

let rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));

type FileType =
    | SingleFile of string
    | Directory of string
    
let parsePath (path: string): FileType  =
    match path with
    | p when File.Exists(p) ->
        Directory path
    | p when Directory.Exists(p) ->
        SingleFile path
    | _ ->
        failwith "Path entered is not a file or directory"
