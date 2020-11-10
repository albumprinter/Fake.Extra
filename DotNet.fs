namespace Albelli

open Fake

    module DotNet =

        let private addToPath (directory : string) : unit =
            let pathvar = System.Environment.GetEnvironmentVariable("PATH")
            let fullPath = directory |> System.IO.Path.GetFullPath
            let separator = if EnvironmentHelper.isLinux then ":" else ";";
            let value  = pathvar + separator + fullPath;
            let target = System.EnvironmentVariableTarget.Process
            System.Environment.SetEnvironmentVariable("PATH", value, target)

        let projectHasLambdaTools (projectPath : string) : bool =
            System.IO.File.ReadAllText projectPath
            |> fun x -> x.Contains "<AWSProjectType>Lambda</AWSProjectType>"

        let packageProjectAsLambdaUsingGlobalTools (lambdaFramework : string) (outputFolder : string) (projectPath : string) : unit =
            let projectDirectory = System.IO.Path.GetDirectoryName projectPath
            let projectName = System.IO.Path.GetFileName projectDirectory
            let outputFile = outputFolder </> (projectName + ".zip") |> System.IO.Path.GetFullPath
            sprintf "lambda package --output-package %s --configuration Release --framework %s --project-location %s" outputFile lambdaFramework projectDirectory
            |> DotNetCli.RunCommand id

        let private getFrameworkFromProject (projectPath : string) : string =
            let xPath = "/Project/PropertyGroup/TargetFramework/text()"
            projectPath
                  |> System.IO.File.ReadAllText
                  |> XMLDoc
                  |> XPathValue xPath []

        let packageProjectAsLambdaDefaultFrameworkUsingGlobalTools (outputFolder : string) (projectPath : string) : unit =
            let framework = getFrameworkFromProject projectPath
            packageProjectAsLambdaUsingGlobalTools framework outputFolder projectPath

        let installGlobalToolPackage (package : string) (version : string) : unit =
            sprintf "tool install -g %s --version %s" package version
            |> DotNetCli.RunCommand id

        let installToolPackage (package : string) (version : string) (directory : string) : unit =
            sprintf "tool install %s --version %s --tool-path %s" package version directory
            |> DotNetCli.RunCommand id
            addToPath(directory)
