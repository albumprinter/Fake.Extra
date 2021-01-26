// In order for this module to work, the following dlls should be included in the main script:
// #r "AWSSDK.Core.dll"
// #r "AWSSDK.SimpleSystemsManagement.dll"

namespace Albelli

open Fake
open Fake.Core
open Amazon.SimpleSystemsManagement

    module ParameterStore =
        let private getParametersStoreParameters() =
            let parameterStorePrefix = "/build/variables"
            let parameterStorePrefixTrailingSlash = parameterStorePrefix + "/"
            use client = new AmazonSimpleSystemsManagementClient()

            let createRequest nextToken =
              Model.GetParametersByPathRequest(
                  WithDecryption = true,
                  Path = parameterStorePrefix,
                  MaxResults = 10,
                  Recursive = true,
                  NextToken = nextToken)

            let getNextResponse token =
                let response =
                    createRequest token
                    |> client.GetParametersByPathAsync |> Async.AwaitTask |> Async.RunSynchronously

                let parameters =
                    response
                    |> fun x-> x.Parameters
                    |> List.ofSeq

                response.NextToken, parameters


            let parameterStoreParams =
                let rec recursiveParams nextToken acc =
                    let newToken, newParamsList = getNextResponse nextToken
                    if System.String.IsNullOrEmpty(nextToken) then
                        acc @ newParamsList
                    else
                        recursiveParams newToken (acc @ newParamsList)

                let newToken, newParamsList = getNextResponse null
                if System.String.IsNullOrEmpty(newToken) then
                        newParamsList
                else
                        recursiveParams newToken newParamsList

            parameterStoreParams
            |> Seq.map (fun p -> p.Name.Substring(parameterStorePrefixTrailingSlash.Length), p.Value)
            |> Map.ofSeq

        let private parameterStoreParameters() = lazy (
          try
            getParametersStoreParameters()
          with
            | ex ->
                failwithf "Failed to get parameter store parameters %s" ex.Message
                Map.empty
        )

        let parameterStoreVarOrNone var =
            parameterStoreParameters().Value
            |> Map.tryFind var

        let anyVarOrNone var =
            match Environment.environVarOrNone var with
            | None _ -> parameterStoreVarOrNone var
            | x -> x

        let anyVarOrFail var =
            match anyVarOrNone var with
            | None _ -> failwithf "Can't find %s variable anywhere" var
            | Some x -> x
