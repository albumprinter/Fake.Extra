// Credit: https://www.jvandertil.nl/posts/2020-07-09_fakegitversionlocaltool/
// In order for this module to work, the following dlls should be included in the main script:
// nuget Fake.DotNet.Cli
// nuget Fake.Tools.GitVersion
namespace Albelli

open Fake.Core
open Fake.DotNet
open Newtonsoft.Json

    module GitVersionTool =
        let generateProperties() =
            let proc = Command.RawCommand("gitversion", Arguments.Empty)
                         |> CreateProcess.fromCommand
                         |> CreateProcess.withToolType (ToolType.CreateLocalTool())
                         |> CreateProcess.redirectOutput
                         |> Proc.run

            if proc.ExitCode <> 0 then
                failwithf "GitVersion failed with exit code %i and message %s" proc.ExitCode proc.Result.Output

            proc.Result.Output
            |> JsonConvert.DeserializeObject<Fake.Tools.GitVersion.GitVersionProperties>
