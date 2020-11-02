namespace Albelli

open Fake

    module DockerCli =

        let runDockerCommand command =
            let result = Shell.Exec ("docker", command)
            if result <> 0 then failwithf "Docker command failed %s" command