# forge-rcw.file.migration-revit.addon
 
![Platforms](https://img.shields.io/badge/platform-Windows-lightgray.svg)
![.NET](https://img.shields.io/badge/.NET-4.8-blue.svg)
![Revit](https://img.shields.io/badge/Revit-2022-blue.svg)

![Visual-Studio](https://img.shields.io/badge/Visual%20Studio-2019-green.svg)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v2-green.svg)](http://developer.autodesk.com/)


![Advanced](https://img.shields.io/badge/Level-Advanced-red.svg)
[![MIT](https://img.shields.io/badge/License-MIT-blue.svg)](http://opensource.org/licenses/MIT)


# Description
The Revit addon Sample is based on the the project from SDK sample(Samples\CloudAPISample\CS), it intends for demonstrating usage of Revit Cloud API. In this sample, we provided the workflow to demostrate migrating Revit RCW model from `BIM 360 Team` to `BIM 360 Docs`.

The sample addon includes the features as follow:
1. Access all the contents within BIM 360 Team and Docs by logging with Autodesk Account.
2. Download the Revit models to specified local folder from BIM 360 Team.
3. Selected the targeted folder by navigating from BIM 360 Docs, and upload the Revit models from local folder to the targeted folder on BIM 360 Docs.
4. Reload the links to the correct model in the cloud.

# Thumbnail
![thumbnail](/thumbnail.png)

# Demo
[![https://youtu.be/i_pmWSXhRec](http://img.youtube.com/vi/i_pmWSXhRec/0.jpg)](https://youtu.be/i_pmWSXhRec "Revit RCW Addon Sample")


# Prerequisites
- Visual Studio (Windows).
- .NET Framework basic knowledge with C#
- Revit API knowledge.

# Running locally
- For using this sample, you need an Autodesk developer credentials. Visit the [Forge Developer Portal](https://developer.autodesk.com), sign up for an account, then [create an app](https://developer.autodesk.com/myapps/create). For this new app, use http://localhost:3000/api/forge/callback/oauth as Callback URL. Finally take note of the Client ID and Client Secret. 

- Connect your Forge App to a Specific BIM 360 Account, follow the [tutorial](https://forge.autodesk.com/en/docs/bim360/v1/tutorials/getting-started/get-access-to-account/)

- Download the repository, open `RevitCloudSample.sln` Solution on Visual Studio. The build process should download the required packages (**Autodesk.Forge** and dependencies). Compile and build the project.

- Setup the environment variables of your Forge App key following the steps, please DO keep the key under your user environment, not expose to others, we are still evaluating the best practice to keep the Forge App Keys.
    1. From the desktop, right-click the very bottom-left corner of the screen to get the Power User Task Menu.
    2. Click System from the Power User Task Menu that’s displayed on the screen.
    3. Under the System menu, you need to click the Advanced System Settings.
    4. In the System Properties window, click the Advanced tab, then click the Environment Variables button near the bottom of that tab.
    5. Add the following environment variables:
```
    FORGE_CLIENT_ID = your_client_key
    FORGE_CLIENT_SECRET = your_client_secret
    FORGE_CALLBACK = http://localhost:3000/api/forge/callback/oauth
```
![variables](variables.png) 

- Before running the plugin, since we need to communicate with 3 legged token callback over HTTP and HTTPS, please run the app with `Admin level`, or at a minimum, you need to configure a URL registration and add a Firewall exception for the URL your service will be using. You can configure these settings with the Netsh.exe tool as follow. Please refer [Configuring HTTP and HTTPS](https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN) for details.

```powershell
netsh http add urlacl url=http://+:3000/api/forge/callback/oauth/ user=DOMAIN\user
```

# Packages 3rd party libraries used
- The app use [NuGet](https://api.nuget.org/v3/index.json) to manage all the packages
- The [Autodesk.Forge](https://www.nuget.org/packages/Autodesk.Forge/) packages is included by default
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [NLog](https://nlog-project.org/)
- [RestSharp](http://restsharp.org/)


# Further Reading
**Documentation:**
- [Data Management API](https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/)
- [BIM 360 API](https://developer.autodesk.com/en/docs/bim360/v1/overview/) and [App Provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps)
- [Revit SDK](https://www.revitapidocs.com/)


# Tips & Tricks
- Before running the plugin, since we need to communicate with 3 legged token callback over HTTP and HTTPS. At a minimum, you want to configure a URL registration and add a Firewall exception for the URL your service will be using. You can configure these settings with the Netsh.exe tool as follow. 
```powershell
netsh http add urlacl url=http://+:3000/api/forge/callback/oauth/ user=DOMAIN\user
```
Please refer [Configuring HTTP and HTTPS](https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN) for details.

# Limitation & Known issue
- Currently,  the tool support downing Revit models from BIM 360 Team, you can specify a folder, and all the files under the folder will be downloaded. But there is no sign to notify when all the files are downloaded, please check the local folder to make sure all the files are fully downloaded before uploading to BIM 360 Docs.   

- After logging with user account, the app will iterate all the projects and folders from BIM 360 Team and Docs, you will see the projects|folders show up gradually, be a little patient if you don't see the folder that you are interested in.

- The 3 legged token will be expired in 30 minutes, the refresh token is not kept and made to refresh the access token automatically, you need to login again if the token is expired, feel free to improve this feature.

- Please DO keep the Forge App key under your user environment variable, not expose to others. We are still evaluating the best practice to keep the Forge App Keys, may change the way if any better solution.

# License
- This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE.md) file for full details.


# Written by
- Zhong Wu [@johnonsoftware](https://twitter.com/johnonsoftware), [Forge Partner Development](http://forge.autodesk.com)
- Based on the original sample `CloudAPISample` under Revit SDK by Will Gu, Software Developer, Autodesk.
