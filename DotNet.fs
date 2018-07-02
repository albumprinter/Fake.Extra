namespace Albelli

open Fake

    module DotNet = 
        let projectHasLambdaTools (projectPath : string) : bool = 
            System.IO.File.ReadAllText projectPath
            |> fun x -> x.Contains "Amazon.Lambda.Tools"

        let packageProjectAsLambda lambdaFramework outputFolder projectPath =
            let projectDirectory = System.IO.Path.GetDirectoryName projectPath
            let projectName = System.IO.Path.GetFileNameWithoutExtension projectDirectory
            let outputFile = outputFolder </> (projectName + ".zip") |> System.IO.Path.GetFullPath
            sprintf "lambda package --output-package %s --configuration Release --framework %s" outputFile lambdaFramework
            |> DotNetCli.RunCommand (fun o -> { o with WorkingDir = projectDirectory } )
