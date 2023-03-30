using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.IO;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using Apps.GoogleDrive.Models.Requests;
using Apps.GoogleDrive.Models.Responses;
using Apps.GoogleDrive.Dtos;
using System;

namespace Apps.GoogleDrive
{
    [ActionList]
    public class Actions
    {
        [Action("Get all items details", Description = "Get all items(files/folders) details")]
        public GetAllItemsResponse GetAllItemsDetails(AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] GetAllItemsRequest input)
        {
            var client = GetGoogleDriveClient(authenticationCredentialsProvider.Value);
            var filesList = client.Files.List().Execute();
            var filesDetails = new List<ItemsDetailsDto>();
            foreach (var file in filesList.Files)
            {
                filesDetails.Add(new ItemsDetailsDto()
                {
                    Name = file.Name,
                    Type = file.MimeType.Equals("application/vnd.google-apps.folder") ? "folder" : "file"
                });
            }

            return new GetAllItemsResponse()
            {
                ItemsDetails = filesDetails
            };
        }

        [Action("Get file", Description = "Get file by Id")]
        public GetFileResponse GetFile(AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] GetFileRequest input)
        {
            var client = GetGoogleDriveClient(authenticationCredentialsProvider.Value);
            var file = client.Files.Get(input.FileId);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                file.Download(stream);
                data = stream.ToArray();
            }
            var fileMetadata = file.Execute();
            return new GetFileResponse()
            {
                Name = fileMetadata.Name,
                Data = data
            };
        }

        [Action("Upload file", Description = "Upload file")]
        public void UploadFile(AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] UploadFileRequest input)
        {
            var client = GetGoogleDriveClient(authenticationCredentialsProvider.Value);
            var body = new Google.Apis.Drive.v3.Data.File();
            body.Name = input.Filename;
            body.Parents = new List<string>() { input.ParentFolderId };

            using (var stream = new MemoryStream(input.File))
            {
                var request = client.Files.Create(body, stream, null);
                request.Upload();
            }
        }

        [Action("Delete item", Description = "Delete item(file/folder) by id")]
        public void DeleteItem(AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] DeleteItemRequest input)
        {
            var client = GetGoogleDriveClient(authenticationCredentialsProvider.Value);
            client.Files.Delete(input.ItemId).Execute();
        }

        [Action("Create folder", Description = "Create folder")]
        public void CreateFolder(AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] CreateFolderRequest input)
        {
            var client = GetGoogleDriveClient(authenticationCredentialsProvider.Value);
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = input.FolderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { input.ParentFolderId }
            };
            var request = client.Files.Create(fileMetadata);
            request.Execute();
        }

        private DriveService GetGoogleDriveClient(string serviceAccountConfString)
        {
            string[] scopes = { DriveService.Scope.Drive };
            ServiceAccountCredential? credential = GoogleCredential.FromJson(serviceAccountConfString)
                                                  .CreateScoped(scopes)
                                                  .UnderlyingCredential as ServiceAccountCredential;
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Blackbird"
            });
            return service;
        }
    }
}
