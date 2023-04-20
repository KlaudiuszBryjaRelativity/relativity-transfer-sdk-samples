# Relativity TransferSDK Samples
---

# Introduction

This project showcases straightforward integration scenario with `TransferSDK`.

##### `TransferSDK` lets you:
- Upload single directory or file to selected destination in Relativity fileshare
- Upload single directory to selected destination in legacy fileshare
- Track overall progress of the transfer
- Track progress of individual items that have been transferred, failed or skipped
- Get detailed transfer report
- Setup one of three predefined retry policies - No Retry, Linear, Exponential 
- Assing an exclusion policy to filter out unwanted files

Library supports `.NETStandard 2.0`, which means it is cross-platform. You can run it on Windows or Linux!

---
## Authentication
 - First we have to [create Transfer Client](https://link_to_implementation.com)] object via provided builder, which is used to manage our transfers.
 - TransferSKD uses a bearer token in order to authenticate the transfer.
 - The token is passed to the Transfer Client via [implementation](https://link_to_implementation.com) of `IRelativityAuthenticationProvider` interface. 
 - In order to get the token, we use [Bearer token authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm#_Bearer_token_authentication), which requires us to provide user OAuth2 client id and client secret. (see [Sample1](https://sample1_link_placeholder.com), [BearerTokenRetriever](https://retriever_class_link_placeholder.com))
    -  Remember that token can become obsolete after some time, so you have to guarantee that `AuthenticationProvider` always returns a valid token.
 - We can read the id and secret from Relativity instance. Go to `Oauth2 Client` tab, find our user in `Context User` column, and after cliking it you can read the Id value and secret. 
     - Keep in mind that secret is valid only limited period of time (8 hours by default), so before copy it is good to regenerate it first.
 - We can also use a [REST service](https://platform.relativity.com/10.3/Content/Authentication/OAuth2_clients.htm#_OAuth2_Client_Manager_REST_service) to automatically refresh the client secret. In order to do that we use [Basic authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm#_Basic_authentication) (see [Sample2](https://sample2_link_placeholder.com), [OauthClientManager](https://link_placeholder.com)) 
    - Warning - this type of authentication will become deprecated in Relativity at the end of 2023. 
 - We strongly recommend to implement your own authentication mechanism which best suits your needs. Here are some helpful links: 
    - [Relativity REST API authentication](https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm)
    - [OAuth2 clients](https://platform.relativity.com/10.3/Content/Authentication/OAuth2_clients.htm#_OAuth2_Client_Manager_REST_service)
    - [Authentication](https://help.relativity.com/RelativityOne/Content/Relativity/Authentication/Authentication.htm)

---
## Running the sample

- Sample is a regular Visual Studio solution (we recommend using Visual Studio 2019 or 2022). You need .NET 5 or higher to compile it.
 - When you build and start the application for the first time, application will ask you for several most needed variables, and also validate some of them by regenerating client secret. 
 - You are also able to modify these variables at runtime. They are stored (except the password) in application's `*.config` file, in the `bin` folder. 
 - If you want to have them set permanently whenever you rebuild the solution, put them into the `App.config` file from the repository
    - Observation: From my experience (in VS 2022), to apply `App.config` file changes into the `bin` config file, rebuilding the solution wasn't enough, I had to also manually remove the `obj` folder.
 - The settings you have to fill up at application start up:
    - `ClientName` - This is required to identify the owner of a job. Can be any string. It is already set in `App.config` so you won't be asked for it until you remove it from config.
    - `RelativityOneInstanceUrl`- the URL to your Relativity instance. Example: `https://contoso.relativity.one`
    - `RelativityFileshareRootPath` - The root of the Realtivity fileshare. Example: `\\files.contoso.pod.r1.kcura.com\contoso`
        - Note: This value can be taken from Relativity. Find `Servers` tab, filter it by `Fileshare`, search for the fileshare you wish to execute a transfer, and copy its `UNC` path value **without** the `\Files\` suffix.
    - `FileshareRelativeDestinationPath` - The location where the files are transferred to, relative to the root of the fileshare. This is already set to `Temp\TransferSDK-Sample`, so you won't be asked for it.
        - **Important!**: the upload elements will be put in the `Temp\TransferSDK-Sample\[transferid]` folder, where `transferid` is an unique transfer identifier
        - Ensure that provided path doesn't have additional `\` and is valid (exists)
        - The path must be rooted in one of the core folders that reside on the fileshare (like Files, Temp, ARM, etc.)
        - You can modify this value and observe the transfer result (in RelativityOne Staging Explorer for example).
     - `DefaultSourceFilePath` - The default file to upload. 
        - When application asks you for source file, just leave it blank and default path will be taken.
        - You can leave it empty, it will be updated when first asked.
     - `DefaultSourceDirectoryPath` - The default directory to upload. When application asks for source directory, just leave it blank and default path will be taken.
        - When application asks you for source directory, just leave it blank and default path will be taken.
        - You can leave it empty, it will be updated when first asked.
     - `ClientLogin` - your Relativity login.
     - `ClentOauth2Id` - your Relativity OAuthClient id, see `Authentication` section for more info
     - `ClientPassword` - your Relativity password.
        - Application won't automatically store the password in `App.config` file, so you will be asked for it whenever it is needed to authorize the operation. But you can place it in App.config by yourself, under `ClientPassword` section. Then you won't be asked again for it. 
 - After providing the settings above, application will try to refresh the client secret. If `RelativityOneInstanceUrl` and client's `login`, `password` and `id` were correct, the secret should be updated, and you can run the samples just by clicking its number. 
 - Some of the samples will run without any additional input (i.e. they will autogenerate test files for upload in temporary folder), some will require additional data from you.
 - When you'll start application again, the settings provided before should be stored. You will be asked only for a password. 
 - In order to edit the settings above you have 3 options: 
    - during application runtime via provided console interface
    - by modifying application *.config file in bin folder.
    - by rebuilding the application (with cleaning the `obj` folder), then the settings should be restored to the same as in `App.config`
 - If you want to be prompted for the settings every time you make the operation (i.e. you want to upload data to different location every time you start the sample), then just keep the required settings empty or whitespace.

---    
# Samples
 ##### Examples structure:
- Each sample is numbered. The number of a sample is included in the file and a class name.
- All code presenting a particular example is contained only in a single file (if additional classes are needed for the example they are defined as private within the example) .
- Usually all necessary inputs are taken at the beginning of the example using `ConsoleHelper` class. 
- `ConsoleHelper` prompts a user for input only when the input was not setup before. That means after initial setup some samples can work without prompting user at all. 
- Sample code contains accurate comments describing the flow.


| Sample name | .Net |
| ------ | ------ |
| Sample1_BearerTokenAuthentication | [Sample1_BearerTokenAuthentication](link) |
| Sample2_BasicCredentialsAndBearerAuthentication | [Sample2_BasicCredentialsAndBearerAuthentication](link) |
| Sample3_SettingUpProgressHandler | [Sample3_SettingUpProgressHandler](link) |
| Sample4_UploadSingleFile | [Sample4_UploadSingleFile](link) |
| Sample5_UploadDirectory | [Sample5_UploadDirectory](link) |
| Sample6_UploadDirectoryWithCustomizedRetryPolicy | [Sample6_UploadDirectoryWithCustomizedRetryPolicy](link) |
| Sample7_UploadDirectoryWithExclusionPolicy | [Sample7_UploadDirectoryWithExclusionPolicy](link) |

---
# Errors 

