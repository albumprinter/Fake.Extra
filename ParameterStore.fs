// In order for this module to work, the following dlls should be included in the main script:
// #r "AWSSDK.Core.dll"
// #r "AWSSDK.SimpleSystemsManagement.dll"

namespace Albelli

open Fake
open Amazon.SimpleSystemsManagement

    module ParameterStore =
        let private parameterStoreParameters() = lazy (
          let parameterStorePrefix = "/build/variables"
          let parameterStorePrefixTrailingSlash = parameterStorePrefix + "/"
          use client = new AmazonSimpleSystemsManagementClient()

          Model.GetParametersByPathRequest (
                      WithDecryption = true,
                      Path = parameterStorePrefix,
                      MaxResults = 10, // should be more than enough for this script
                      Recursive = true)
          |> client.GetParametersByPath
          |> fun x-> x.Parameters
          |> Seq.map (fun p -> p.Name.Substring(parameterStorePrefixTrailingSlash.Length), p.Value)
          |> Map.ofSeq
        )

        let parameterStoreVarOrNone var = 
            parameterStoreParameters().Value
            |> Map.tryFind var

        let anyVarOrNone var = 
            match environVarOrNone var with
            | None _ -> parameterStoreVarOrNone var
            | x -> x

        let anyVarOrFail var = 
            match anyVarOrNone var with
            | None _ -> failwithf "Can't find %s variable anywhere" var
            | Some x -> x
