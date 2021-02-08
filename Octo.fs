namespace Albelli

open Fake.Core
open Fake.DotNet

    module OctoTool =
          let pack (id : string) (version : string) (basePath : string) (outFolder : string) =
            let args = 
                Arguments.Empty
                |> Arguments.append ["pack"]
                |> Arguments.appendNotEmpty "--id" id
                |> Arguments.appendNotEmpty "--version" version
                |> Arguments.appendNotEmpty "--basepath" basePath
                |> Arguments.appendNotEmpty "--outFolder" outFolder
                |> Arguments.toArray

            let proc =
                CreateProcess.fromRawCommand "octo" args
                |> CreateProcess.withToolType (ToolType.CreateLocalTool())
                |> CreateProcess.redirectOutput
                |> Proc.run

            if proc.ExitCode <> 0 then failwithf "Octo failed with exit code %i and message %s" proc.ExitCode proc.Result.Output