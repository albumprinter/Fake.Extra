namespace Albelli

open Fake
open Fake.DotNet
open Fake.Core
open Fake.Core.Xml
open Fake.IO.FileSystemOperators

    module DotNet =

        let projectHasLambdaTools (projectPath : string) : bool =
            System.IO.File.ReadAllText projectPath
            |> fun x -> x.Contains "<AWSProjectType>Lambda</AWSProjectType>"

        let isEcsMainProject (projectPath : string) : bool =
            System.IO.File.ReadAllText projectPath
            |> fun x -> x.Contains "<PropertyGroup Label=\"EcsMainProject\" />"

        let packageProjectAsLambda lambdaFramework outputFolder (projectPath : string) =
            let projectDirectory = System.IO.Path.GetDirectoryName projectPath
            let projectName = System.IO.Path.GetFileName projectDirectory
            let outputFile = outputFolder </> (projectName + ".zip") |> System.IO.Path.GetFullPath
            let args = 
                Arguments.Empty
                |> Arguments.append ["package"]
                |> Arguments.appendNotEmpty "--output-package" outputFile
                |> Arguments.appendNotEmpty "--configuration" "Release"
                |> Arguments.appendNotEmpty "--framework" lambdaFramework
                |> Arguments.appendNotEmpty "--project-location" projectDirectory
                |> Arguments.toArray
            let proc =
                CreateProcess.fromRawCommand "lambda" args
                |> CreateProcess.withToolType (ToolType.CreateLocalTool())
                |> CreateProcess.redirectOutput
                |> Proc.run
            if proc.ExitCode <> 0 then failwithf "dotnet lambda package failed with exit code %i and message %s" proc.ExitCode proc.Result.Output

        let private getFrameworkFromProject projectPath =
            let xPath = "/*[local-name()='Project']/*[local-name()='PropertyGroup']/*[local-name()='TargetFramework']"
            projectPath
            |> Fake.Core.Xml.loadDoc
            |> Fake.Core.Xml.selectXPathValue xPath []

        let packageProjectAsLambdaDefaultFramework outputFolder projectPath =
            let framework = getFrameworkFromProject projectPath
            packageProjectAsLambda framework outputFolder projectPath      