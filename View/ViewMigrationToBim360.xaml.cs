//
// (C) Copyright 2003-2020 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using Revit.SDK.Samples.CloudAPISample.CS.Coroutine;
using Revit.SDK.Samples.CloudAPISample.CS.Migration;
using Revit.SDK.Samples.CloudAPISample.CS.Forge;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Revit.SDK.Samples.CloudAPISample.CS.View
{
    /// <summary>
    ///    Interaction logic for ViewMigrationToBim360.xaml
    /// </summary>
    public partial class ViewMigrationToBim360 : UserControl
    {
        /// <summary>
        ///    The view for <see cref="MigrationToBim360" />
        /// </summary>
        public ViewMigrationToBim360(MigrationToBim360 sampleContext)
        {
            DataContext = sampleContext;
            InitializeComponent();
        }

        /// <summary>
        ///    Update progress for uploading process.
        /// </summary>
        /// <param name="status">Text prompt to indicate the process</param>
        /// <param name="progress">The percentage progress, from 0 to 100</param>
        public void UpdateUploadingProgress(string status, int progress)
        {
            lbUploadStatus.Content = status;
            pbUploading.Value = progress;
        }


        /// <summary>
        ///    Update progress for reloading process
        /// </summary>
        /// <param name="status">Text prompt to indicate the process</param>
        /// <param name="progress">The percentage progress, from 0 to 100</param>
        public void UpdateReloadingProgress(string status, int progress)
        {
            lbReloadStatus.Content = status;
            pbReloading.Value = progress;
        }

        private void OnBtnBrowseDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    tbLocalFolder.Text = fbd.SelectedPath;
            }
        }


        private void OnBtnUpload_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem outputFolder = this.treeDataManagementOutput.SelectedItem as TreeViewItem;

            if (null == outputFolder )
            {
                MessageBox.Show("Please select target output folder under BIM 360 Docs.");
                return;
            }

            string[] parameters = outputFolder.Tag.ToString().Split('/');
            string resourceType = parameters[parameters.Length - 2];
            if( resourceType != "folders")
            {
                MessageBox.Show("Please select target output folder under BIM 360 Docs.");
                return;
            }

            string sProjectId = parameters[parameters.Length - 3].Replace("b.", "");
            string sFolderId = parameters[parameters.Length - 1];
            string localDir = tbLocalFolder.Text;

            // get the account Id
            string sAccountId = string.Empty;
            var item = outputFolder.Parent as TreeViewItem;
            while (item != null)
            {
                parameters = item.Tag.ToString().Split('/');
                resourceType = parameters[parameters.Length - 2];
                if( resourceType == "hubs")
                {
                    sAccountId = parameters[parameters.Length - 1].Replace("b.", "");
                    break;
                }
                else
                {
                    item = item.Parent as TreeViewItem; 
                }
            }

            if (DataContext is MigrationToBim360 sampleContext)
            {
                var folderLocation = new FolderLocation();
                folderLocation.Name = @"Folder";
                folderLocation.Urn = sFolderId;

                var rule = new MigrationRule();
                rule.Pattern = @"*.*";
                rule.Target = folderLocation;

                var model = sampleContext.Model;
                model.Rules.Clear();
                model.Rules.Add(rule);

                // Upload local models to target project
                if (Guid.TryParse(sAccountId, out var accountId) && Guid.TryParse(sProjectId, out var projectId))
                    CoroutineScheduler.StartCoroutine(sampleContext.Upload(localDir, accountId, projectId,
                       sampleContext.Model.Rules));
            }
        }

        private void OnBtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem outputFolder = this.treeDataManagementOutput.SelectedItem as TreeViewItem;
            if (null == outputFolder)
            {
                MessageBox.Show("Please select target output folder under BIM 360 Docs.");
                return;
            }

            string[] parameters = outputFolder.Tag.ToString().Split('/');
            string resourceType = parameters[parameters.Length - 2];
            if (resourceType != "folders")
            {
                MessageBox.Show("Please select target output folder under BIM 360 Docs.");
                return;
            }

            string sProjectId = parameters[parameters.Length - 3].Replace("b.", "");
            string sFolderId = parameters[parameters.Length - 1];
            string localDir = tbLocalFolder.Text;

            if (DataContext is MigrationToBim360 sampleContext)
            {
                var folderLocation = new FolderLocation();
                folderLocation.Name = @"Folder";
                folderLocation.Urn = sFolderId;

                var rule = new MigrationRule();
                rule.Pattern = @"*.*";
                rule.Target = folderLocation;

                var model = sampleContext.Model;
                model.Rules.Clear();
                model.Rules.Add(rule);

                if (Guid.TryParse(sProjectId, out var projectId))
                    CoroutineScheduler.StartCoroutine(sampleContext.ReloadLinks(localDir, projectId));
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public void GenerateTokenCallback( )
        {
            if (ThreeLeggedToken.TokenInitialized)
            {
                // change the button
                btnSignIn.Visibility = Visibility.Hidden;
                btnSignout.Visibility = Visibility.Visible;
                PrepareUserData();
            }
            else
            {
                btnSignIn.Visibility = Visibility.Visible;
                btnSignout.Visibility = Visibility.Hidden;
            }
        }



        private void OnBtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ThreeLeggedToken.TokenData tokenData = new ThreeLeggedToken.TokenData();
            tokenData.callback = new ThreeLeggedToken.NewBearerDelegate(GenerateTokenCallback);
            tokenData.control = this;

            ThreeLeggedToken.GenerateToken(tokenData);
        }

        private void OnBtnLogout_Click(object sender, RoutedEventArgs e)
        {
            //TBD:

        }

        private async void OnBtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (null == this.treeDataManagement.SelectedItem)
            {
                return;
            }
            TreeViewItem curSel = this.treeDataManagement.SelectedItem as TreeViewItem;
            string[] parameters = curSel.Tag.ToString().Split('/');
            string resourceType = parameters[parameters.Length - 2];
            string resourceId = parameters[parameters.Length - 1];

            if (resourceType == "folders")
            {
                string projectId = parameters[parameters.Length - 3];
                await downloadFilesFromFolder(projectId, resourceId, tbLocalFolder.Text);
            }
            else
            {
                MessageBox.Show("Please select a folder of BIM 360 Team");
                return;
            }
            return;
        }


        private async void PrepareUserData( )
        {
            // show user name
            User user = await User.GetUserProfileAsync();
            btnSignout.Content = string.Format("User: {0} {1}", user.FirstName, user.LastName);

            refreshTreeViewControl(treeDataManagement, true);
            refreshTreeViewControl(treeDataManagementOutput, false);
        }


        private async void refreshTreeViewControl(System.Windows.Controls.TreeView control, bool isBimTeam )
        {            
            control.Items.Clear();
            TreeViewItem rootItem = new TreeViewItem();
            rootItem.Header = isBimTeam? "BIM 360 Team" : "BIM 360 Docs";
            control.Items.Add(rootItem);

            // show hubs tree
            dynamic hubs = await DataManagement.GetHubsAsync();
            foreach (var hub in hubs)
            {
                if (isBimTeam && hub.Type == "bim360hubs")
                    continue;

                if (!isBimTeam && hub.Type != "bim360hubs")
                    continue;

                TreeViewItem hubItem = new TreeViewItem();
                hubItem.Header = hub.Text;
                hubItem.Tag = hub.ID;
                rootItem.Items.Add(hubItem);

                addTreeViewItems(hubItem );
            }
            return;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootItem"></param>
        /// <returns></returns>
        private async void addTreeViewItems( TreeViewItem rootItem)
        {
            if (rootItem == null )
                return;

            string[] parameters = rootItem.Tag.ToString().Split('/');
            string resourceType = parameters[parameters.Length - 2];
            string resourceId = parameters[parameters.Length - 1];

            switch (resourceType)
            {
                case "hubs":
                    IList<DataManagement.Item> projects = null;
                    try
                    {
                        projects = await DataManagement.GetProjectsAsync(resourceId);
                    }
                    catch( Exception e)
                    {
                        Console.WriteLine(e);
                        break;
                    }
                    foreach(var project in projects )
                    {
                        TreeViewItem projectItem = new TreeViewItem();
                        projectItem.Tag = project.ID;
                        projectItem.Header = project.Text;
                        rootItem.Items.Add(projectItem);
                        addTreeViewItems(projectItem);
                    }
                    break;
                case "projects":
                    string hubId = parameters[parameters.Length - 3];
                    IList<DataManagement.Item> folders = null;
                    try
                    {
                        folders = await DataManagement.GetTopFoldersAsync(hubId, resourceId);
                    } // User has admin permission but don't have Docs permision will fail here.
                    catch( Exception e) 
                    {
                        Console.Write(e);
                        break;
                    }
                    foreach ( var folder in folders)
                    {
                        TreeViewItem folderItem = new TreeViewItem();
                        folderItem.Tag = folder.ID;
                        folderItem.Header = folder.Text;
                        rootItem.Items.Add(folderItem);
                        addTreeViewItems(folderItem);
                    }
                    break;

                case "folders":
                    string projectId = parameters[parameters.Length - 3];
                    IList<DataManagement.Item> contents = null;
                    try
                    {
                        contents = await DataManagement.GetFolderContentsAsync(projectId, resourceId);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                        break;
                    }
                    foreach ( var content in contents)
                    {
                        if (content.Type != "folders")
                            continue;

                        TreeViewItem contentItem = new TreeViewItem();
                        contentItem.Tag = content.ID;
                        contentItem.Header = content.Text;
                        rootItem.Items.Add(contentItem);
                        addTreeViewItems(contentItem);
                    }
                    break;
            }
            return;
        }


       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="folderId"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        private async Task downloadFilesFromFolder( string projectId, string folderId, string outputFolder )
        {
           IList<DataManagement.Item> contents = await DataManagement.GetFolderContentsAsync(projectId, folderId);
           foreach( var item in contents)
           {
               string[] parameters = item.ID.Split('/');
               string resourceId = parameters[parameters.Length - 1];
               if ( item.Type == "folders")
               {
                   await downloadFilesFromFolder(projectId, resourceId, Path.Combine(outputFolder, item.Text));
               }
               if( item.Type == "items")
               {
                   DataManagement.DownloadFile( projectId, resourceId, outputFolder );
               }
           }
           return;
        }
    }
}