namespace Albelli

open Fake
open Fake.Core

       module DockerCli =

        let login (username: string) (password: string) (url: string) =
            let args = 
                Arguments.Empty
                |> Arguments.append ["login"]
                |> Arguments.appendNotEmpty "-u" username
                |> Arguments.appendNotEmpty "-p" password 
                |> Arguments.append [url]
                |> Arguments.toArray
            let proc =
                CreateProcess.fromRawCommand "docker" args
                |> CreateProcess.redirectOutput
                |> Proc.run
            if proc.ExitCode <> 0 then failwithf "docker login failed with exit code %i and message %s" proc.ExitCode proc.Result.Output
        
        let build (dockerImageTag: string) (outputDirectory: string) =
            let args = 
                Arguments.Empty
                |> Arguments.append ["build"]
                |> Arguments.appendNotEmpty "-f" "Dockerfile"
                |> Arguments.appendNotEmpty "-t" dockerImageTag 
                |> Arguments.append [outputDirectory]
                |> Arguments.toArray
            let proc =
                CreateProcess.fromRawCommand "docker" args
                |> CreateProcess.redirectOutput
                |> Proc.run
            if proc.ExitCode <> 0 then failwithf "docker build failed with exit code %i and message %s" proc.ExitCode proc.Result.Output
       
        let push (dockerImageTag: string) =
            let args = 
                Arguments.Empty
                |> Arguments.append ["push"; dockerImageTag]
                |> Arguments.toArray
            let proc =
                CreateProcess.fromRawCommand "docker" args
                |> CreateProcess.redirectOutput
                |> Proc.run
            if proc.ExitCode <> 0 then failwithf "docker push failed with exit code %i and message %s" proc.ExitCode proc.Result.Output