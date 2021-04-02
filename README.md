# Revit-Cloud.WorkSharing-Sample
 






# forge-bim360.project.setup.tool

![Platforms](https://img.shields.io/badge/platform-Windows-lightgray.svg)
![.NET](https://img.shields.io/badge/.NET-4.6-blue.svg)

[![Data-Management](https://img.shields.io/badge/Data%20Management-v2-green.svg)](http://developer.autodesk.com/)


![Advanced](https://img.shields.io/badge/Level-Advanced-red.svg)
[![MIT](https://img.shields.io/badge/License-MIT-blue.svg)](http://opensource.org/licenses/MIT)



# Description
The Revit RCW Addon Sample is based on the the project from SDK sample(Samples\CloudAPISample\CS), it intends for demonstrating usage of Revit Cloud API. In this sample, we provided the workflow to demostrate migrating an A360 Team model to BIM 360 Docs.

The sample addon includes the features as follow:
1. Access all the contents within BIM 360 Team and Docs by logging with Autodesk Account.
2. Download the Revit models to specified local folder from BIM 360 Teams by navigating folders from BIM 360 projects.
3. Selected the targeted folder bt navigating from BIM 360 Docs, and upload the Revit models from local folder to the specified targeted folder on BIM 360 Docs.
4. Reload the links to the correct model in the cloud.

# Thumbnail
![thumbnail](/thumbnail.png)

# Demo
[![https://youtu.be/i_pmWSXhRec](http://img.youtube.com/vi/i_pmWSXhRec/0.jpg)](https://youtu.be/i_pmWSXhRec "Revit RCW Addon Sample")


# Prerequisites
- Visual Studio: Either Community (Windows).
- Revit API knowledge.
- .NET Framework basic knowledge with C#

# Running locally
- For using this sample, you need an Autodesk developer credentials. Visit the [Forge Developer Portal](https://developer.autodesk.com), sign up for an account, then [create an app](https://developer.autodesk.com/myapps/create). 

- Connect your Forge App to a Specific BIM 360 Account, follow the [tutorial](https://forge.autodesk.com/en/docs/bim360/v1/tutorials/getting-started/get-access-to-account/)

- Download the repository, open `RevitCloudSample.sln` Solution on Visual Studio. The build process should download the required packages (**Autodesk.Forge** and dependencies). Compile and build the project.




# Packages 3rd party libraries used
- The app use [NuGet](https://api.nuget.org/v3/index.json) to manage all the packages
- The [Autodesk.Forge](https://www.nuget.org/packages/Autodesk.Forge/) packages is included by default
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [NLog](https://nlog-project.org/)
- [RestSharp](http://restsharp.org/)


# Further Reading
**Documentation:**
- [BIM 360 API](https://developer.autodesk.com/en/docs/bim360/v1/overview/) and [App Provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps)


# Tips & Tricks
- Before running the plugin, since we need to communicate with 3 legged token callback over HTTP and HTTPS. At a minimum, you want to configure a URL registration and add a Firewall exception for the URL your service will be using. You can configure these settings with the Netsh.exe tool as follow. 
```powershell
netsh http add urlacl url=http://+:3000/api/forge/callback/oauth/ user=DOMAIN\user
```
Please refer [Configuring HTTP and HTTPS](https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN) for details.

# Limitation & Known issue
- CSV files need to be prepared with the correct format and required parameters, check [User Guide](BIM360-ProjectSetupTool-UsageGuide.pdf) for details.
- **Copy Folder** only support **Plan** and **Project File** folder and their subfolders.
- Copy folder support copy role permissions with this tool.


# Change History
All the changes will be tracked, please see the [Change History](CHANGELOG.md) file for full details.

# License
This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE.md) file for full details.


# Written by
- Oliver Scharpf, Global Consulting Delivery team, Autodesk.
- Reviewed and maintained by Zhong Wu [@johnonsoftware](https://twitter.com/johnonsoftware), [Forge Partner Development](http://forge.autodesk.com)
