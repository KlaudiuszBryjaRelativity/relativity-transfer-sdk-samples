# Relativity TransferSDK Samples

## Introduction

This project showcases a straightforward integration scenario with `TransferSDK`.

#### `TransferSDK` lets you:
- Upload a single directory or file to the selected destination in Relativity fileshare
- Track the overall progress of the transfer
- Track the progress of individual items that have been transferred, failed, or skipped
- Get a detailed transfer report
- Setup one of three predefined retry policies - No Retry, Linear, Exponential 
- Assing an exclusion policy to filter out unwanted files

The library supports `.NETStandard 2.0`, which means it is cross-platform. You can run it on Windows or Linux!

## Samples
#### Examples structure:
- Each sample is numbered. The number of a sample is included in the file and the class name.
- All code presenting a particular example is contained only in a single file (if additional classes are needed for the example they are defined as private within the example).
- Usually, all necessary inputs are taken at the beginning of the example using `ConsoleHelper` class.
- `ConsoleHelper` prompts a user for input only when the input was not set up before. That means after initial setup some samples can work without prompting the user at all.
- Sample code contains accurate comments describing the flow.


| Sample name | .Net |
| ------ | ------ |
| Sample1_BearerTokenAuthentication | [Sample1_BearerTokenAuthentication](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample1_BearerTokenAuthentication.cs) |
| Sample2_BasicCredentialsAndBearerAuthentication | [Sample2_BasicCredentialsAndBearerAuthentication](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample2_BasicCredentialsAndBearerAuthentication.cs) |
| Sample3_SettingUpProgressHandlerAndPrintingSummary | [Sample3_SettingUpProgressHandlerAndPrintingSummary](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample3_SettingUpProgressHandlerAndPrintingSummary.cs) |
| Sample4_UploadSingleFile | [Sample4_UploadSingleFile](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample4_UploadSingleFile.cs) |
| Sample5_UploadDirectory | [Sample5_UploadDirectory](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample5_UploadDirectory.cs) |
| Sample6_UploadDirectoryWithCustomizedRetryPolicy | [Sample6_UploadDirectoryWithCustomizedRetryPolicy](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample6_UploadDirectoryWithCustomizedRetryPolicy.cs) |
| Sample7_UploadDirectoryWithExclusionPolicy | [Sample7_UploadDirectoryWithExclusionPolicy](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample7_UploadDirectoryWithExclusionPolicy.cs) |
| Sample8_UploadToFilesharePathBasedOnWorkspaceId | [Sample8_UploadToFilesharePathBasedOnWorkspaceId](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample8_UploadToFilesharePathBasedOnWorkspaceId.cs)

## Running the sample

- Sample is a regular Visual Studio solution (it is recommended to use Visual Studio 2019 or 2022)
- During the first application start, it will ask for several most needed settings, and also validate some of them by regenerating the client secret. 
- The settings also can be modified at runtime. They are stored (except the password) in the application's `*.config` file, in the `bin` folder. 
- Whenever the application is rebuilt, the bin `*.config` file is restored to the values from `App.config` file from the repository.
 - It is advised to manually remove the `obj` folder and rebuild the solution in order to apply `App.config` file changes into the `bin` config file.
- Directly modifying the `App.config` file keeps these settings between rebuilds.
- The settings to fill up at application start-up:
    - `ClientName` - This is required to identify the owner of a job. Can be any string.
    - `RelativityOneInstanceUrl`- The URL to the Relativity instance. Example: `https://contoso.relativity.one`
    - `RelativityFileshareRootPath` - The root of the Relativity fileshare. Example: `\\files.contoso.pod.r1.kcura.com\contoso`
        - Note: This value can be taken from Relativity. Find the `Servers` tab, filter it by `Fileshare`, search for the specified one, and copy its `UNC` path value **without** the `\Files\` suffix.
    - `FileshareRelativeDestinationPath` - The location where the files are transferred relative to the root of the fileshare.
        - **Note**: `<transferJobId>` folder is created in provided location.
        - **Note**: Provided path can not have `\` at the beginning.
        - The path must be rooted in one of the core folders that reside on the fileshare (like Files, Temp, etc.)
        - Transfer results can be observed in `RelativityOne Staging Explorer`.
    - `DefaultSourceFilePath`.
        - The application will use this value when an empty path is provided.
    - `DefaultSourceDirectoryPath`.
        - The application will use this value when an empty path is provided.
    - `ClientLogin`.
    - `ClentOauth2Id` - Relativity OAuthClient id, see `Authentication` section for more info.
    - `ClientPassword`.
        - The password is not stored in the `App.config` file by default
        - To store the password the `App.config` must be directly modified, under the `ClientPassword` section.
- There are 3 options to edit the above settings:
    - during application runtime via the provided console interface
    - by modifying the application *.config file in the bin folder.
    - by modifying `App.config` file and rebuilding the application (and cleaning the `obj` folder)
- To be prompted for the settings every time the operation is made, the required settings in the config should be empty.

## Authentication
- First, the [transfer client object](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample1_BearerTokenAuthentication.cs#L36-L39) has to be created, which is used to manage transfers.
- TransferSKD uses a bearer token in order to authenticate the transfer.
- To pass the token, the `IRelativityAuthenticationProvider` [object](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample1_BearerTokenAuthentication.cs#L54) should be [registered](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample1_BearerTokenAuthentication.cs#L37). 
- In order to get the token, [Bearer token authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm#_Bearer_token_authentication) is used, which requires user OAuth2 client id and client secret. (see [Sample1](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample1_BearerTokenAuthentication.cs#L60), [BearerTokenRetriever](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Authentication/BearerTokenRetriever.cs))
    - The token can become obsolete after some time. It's the `AuthenticationProvider` responsibility to always return a valid token.
- Client id and secret can be read from the Relativity instance. It is under the `Oauth2 Client` tab. Appropriate users can be identified by the `Context User` column.
    - The secret is valid for only a limited period of time (8 hours by default), so it is advised to regenerate it before copying.
- Also the [REST service](https://platform.relativity.com/10.3/Content/Authentication/OAuth2_clients.htm#_OAuth2_Client_Manager_REST_service) can be used to automatically refresh the client secret. In order to do that [Basic authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm#_Basic_authentication) (see [Sample2](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Samples/Sample2_BasicCredentialsAndBearerAuthentication.cs), [OauthClientManager](https://github.com/relativitydev/relativity-transfer-sdk-samples/blob/main/Source/Relativity.Transfer.SDK.Sample/Authentication/OAuthClientManager.cs)) is used. 
    - Warning - this type of authentication will become deprecated in Relativity at the end of 2023. 
- It is strongly recommended to implement your own authentication mechanism which best suits your needs. Here are some helpful links: 
    - [Relativity REST API authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm)
    - [OAuth2 clients](https://platform.relativity.com/10.3/Content/Authentication/OAuth2_clients.htm#_OAuth2_Client_Manager_REST_service)
    - [Authentication](https://help.relativity.com/RelativityOne/Content/Relativity/Authentication/Authentication.htm)

## Exceptions 
Some exceptions that can be encountered when using samples and their potential root causes

#### System.Net.WebException: The remote name could not be resolved:
- sample console output: 
```
Exception occurred during execution of the transfer. Look at the inner exception for more details.
Relativity.Transfer.SDK.Interfaces.Exceptions.TransferJobExecutionException: Exception occurred during execution of the transfer. Look at the inner exception for more details. ---> Relativity.Transfer.SDK.Interfaces.Exceptions.UnauthorizedException: Unable to retrieve the authentication token. Check inner exception for details. ---> System.ApplicationException: Failed to retrieve credentials. ---> System.ApplicationException: Failed to retrieve bearer token. ---> System.Net.Http.HttpRequestException: An error occurred while sending the request. ---> System.Net.WebException: The remote name could not be resolved: 'reg-b.r1.kcurdda.com'
```
- Reasons: 
    - Wrong RelativityOneInstanceUrl value. 
    - Relativity instance is not available 

#### Relativity.Transfer.SDK.Interfaces.Exceptions.UnauthorizedException:
- sample console output: 
```
Exception occurred during execution of the transfer. Look at the inner exception for more details.
Relativity.Transfer.SDK.Interfaces.Exceptions.TransferJobExecutionException: Exception occurred during execution of the transfer. Look at the inner exception for more details. ---> Relativity.Transfer.SDK.Interfaces.Exceptions.UnauthorizedException: Unable to retrieve the authentication token. Check inner exception for details. ---> System.InvalidOperationException: API call 'ReadAsync' failed with status code: 'Unauthorized' Details: ''.
```
- Reasons: 
    - wrong password/credentials/client ID

#### Relativity.Transfer.SDK.Interfaces.Exceptions.BackendServiceException: Forbidden
- sample console output: 
```
Exception occurred during execution of the transfer. Look at the inner exception for more details.
Relativity.Transfer.SDK.Interfaces.Exceptions.TransferJobExecutionException: Exception occurred during execution of the transfer. Look at the inner exception for more details. ---> Relativity.Transfer.SDK.Interfaces.Exceptions.BackendServiceException: Forbidden
```
- Reasons: 
    - Wrong FileshareRelativeDestinationPath setting. Ensure there is no `\` at the beginning of the path.
    - Wrong RelativityOneFileshareRoot. Ensure there is no `files` suffix on the path, and the path is correct.

#### Relativity.Transfer.SDK.Interfaces.Exceptions.BackendServiceException: Unsupported Media Type:
- sample console output: 
```
Exception occurred during execution of the transfer. Look at the inner exception for more details.
Relativity.Transfer.SDK.Interfaces.Exceptions.TransferJobExecutionException: Exception occurred during execution of the transfer. Look at the inner exception for more details. ---> Relativity.Transfer.SDK.Interfaces.Exceptions.BackendServiceException: Unsupported Media Type
```
- Reasons: 
    - Wrong RelativityOneInstanceUrl. Ensure value is correct, and it ends with `*com` and there is **NO** `/Relativity/` suffix.
