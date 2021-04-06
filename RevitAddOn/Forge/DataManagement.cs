using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Model;


namespace Revit.SDK.Samples.CloudAPISample.CS.Forge
{
    class DataManagement
    {
        /// <summary>
        /// 
        /// </summary>
        public class Item
        {
            public Item(string id, string text, string type)
            {
                this.ID = id;
                this.Type = type;
                this.Text = text;
            }

            public string ID;
            public string Type;
            public string Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<Item>> GetHubsAsync()
        {
            HubsApi hubsApi = new HubsApi();
            string userAccessToken = ThreeLeggedToken.GetToken();
            hubsApi.Configuration.AccessToken = userAccessToken;

            IList<Item> nodes = new List<Item>();
            var hubs = await hubsApi.GetHubsAsync();
            foreach (KeyValuePair<string, dynamic> hubInfo in new DynamicDictionaryItems(hubs.data))
            {
                string hubType = "hubs";
                switch ((string)hubInfo.Value.attributes.extension.type)
                {
                    case "hubs:autodesk.core:Hub":
                        hubType = "hubs";
                        break;
                    case "hubs:autodesk.a360:PersonalHub":
                        hubType = "personalhub";
                        break;
                    case "hubs:autodesk.bim360:Account":
                        hubType = "bim360hubs";
                        break;
                }
                Item item = new Item(hubInfo.Value.links.self.href, hubInfo.Value.attributes.name, hubType);
                nodes.Add(item);
            }

            return nodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubId"></param>
        /// <returns></returns>
        public static async Task<IList<Item>> GetProjectsAsync(string hubId)
        {
            IList<Item> nodes = new List<Item>();

            ProjectsApi projectsApi = new ProjectsApi();
            string userAccessToken = ThreeLeggedToken.GetToken();
            projectsApi.Configuration.AccessToken = userAccessToken;
            var projects = await projectsApi.GetHubProjectsAsync(hubId);
            foreach (KeyValuePair<string, dynamic> projectInfo in new DynamicDictionaryItems(projects.data))
            {
                string projectType = "projects";
                switch ((string)projectInfo.Value.attributes.extension.type)
                {
                    case "projects:autodesk.core:Project":
                        projectType = "a360projects";
                        break;
                    case "projects:autodesk.bim360:Project":
                        projectType = "bim360projects";
                        break;
                }
                Item projectNode = new Item(projectInfo.Value.links.self.href, projectInfo.Value.attributes.name, projectType);
                nodes.Add(projectNode);
            }

            return nodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static async Task<IList<Item>> GetTopFoldersAsync(string hubId, string projectId)
        {
            IList<Item> nodes = new List<Item>();

            ProjectsApi projectsApi = new ProjectsApi();
            string userAccessToken = ThreeLeggedToken.GetToken();
            projectsApi.Configuration.AccessToken = userAccessToken;
            var folders = await projectsApi.GetProjectTopFoldersAsync(hubId, projectId);
            foreach (KeyValuePair<string, dynamic> folderInfo in new DynamicDictionaryItems(folders.data))
            {
                Item projectNode = new Item(folderInfo.Value.links.self.href, folderInfo.Value.attributes.displayName, "folders");
                nodes.Add(projectNode);
            }

            return nodes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public static async Task<IList<Item>> GetFolderContentsAsync(string projectId, string folderId)
        {
            IList<Item> folderItems = new List<Item>();

            FoldersApi folderApi = new FoldersApi();
            string userAccessToken = ThreeLeggedToken.GetToken();
            folderApi.Configuration.AccessToken = userAccessToken;
            var folderContents = await folderApi.GetFolderContentsAsync(projectId, folderId);
            foreach (KeyValuePair<string, dynamic> folderContentItem in new DynamicDictionaryItems(folderContents.data))
            {
                string displayName = folderContentItem.Value.attributes.displayName;
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    continue;
                }

                Item itemNode = new Item(folderContentItem.Value.links.self.href, displayName, (string)folderContentItem.Value.type);

                folderItems.Add(itemNode);
            }

            return folderItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static async Task<IList<Item>> GetItemVersionsAsync(string projectId, string itemId)
        {
            string userAccessToken = ThreeLeggedToken.GetToken();
            IList<Item> versionsList = new List<Item>();
            ItemsApi itemsApi = new ItemsApi();
            itemsApi.Configuration.AccessToken = userAccessToken;
            var versions = await itemsApi.GetItemVersionsAsync(projectId, itemId);
            foreach (KeyValuePair<string, dynamic> version in new DynamicDictionaryItems(versions.data))
            {
                DateTime versionDate = version.Value.attributes.lastModifiedTime;

                string urn = string.Empty;
                try { urn = (string)version.Value.relationships.derivatives.data.id; }
                catch { urn = "not_available"; } // some BIM 360 versions don't have viewable

                Item itemNode = new Item("/versions/" + urn, versionDate.ToString("dd/MM/yy HH:mm:ss"), "versions");

                versionsList.Add(itemNode);
            }

            return versionsList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="itemId"></param>
        /// <param name="outputFolder"></param>
        public static async void DownloadFile(string projectId, string itemId, string outputFolder)
        {

            string userAccessToken = ThreeLeggedToken.GetToken();
            ItemsApi itemsApi = new ItemsApi();
            itemsApi.Configuration.AccessToken = userAccessToken;
            var item = await itemsApi.GetItemAsync(projectId, itemId);
            string displayNmae = string.Empty;
            try { 
                displayNmae = item.data.attributes.displayName; 
            }
            catch { 
                displayNmae = "no_name"; 
            }

            // get the storage of the item
            string storageUrl = string.Empty;
            foreach (KeyValuePair<string, dynamic> version in new DynamicDictionaryItems(item.included))
            {
                try 
                {
                    storageUrl = (string)version.Value.relationships.storage.meta.link.href; 
                }
                catch
                { 
                    storageUrl = string.Empty;
                    return;
                } 
            }

            string[] parameters = storageUrl.Split('/');
            string bucketKey = parameters[parameters.Length - 3];
            string objectNameFull = parameters[parameters.Length - 1];
            string objectName = objectNameFull.Split('?')[0];

            var objectApi = new ObjectsApi();
            objectApi.Configuration.AccessToken = userAccessToken;
            Stream receiveStream = null;
            try
            {
                receiveStream = objectApi.GetObject(bucketKey, objectName);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return;
            }
            Directory.CreateDirectory(outputFolder);
            FileStream fs = new FileStream( Path.Combine( outputFolder, displayNmae) , FileMode.Create, FileAccess.Write);
            Byte[] bytes = new Byte[100];
            int count = receiveStream.Read(bytes, 0, 100);
            while (count != 0)
            {
                fs.Write(bytes, 0, count);
                count = receiveStream.Read(bytes, 0, 100);
            }
            fs.Close();
            receiveStream.Close();

            return;
        }
    }
}
