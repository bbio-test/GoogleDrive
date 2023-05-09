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
using Blackbird.Applications.Sdk.Common.Actions;

namespace Apps.GoogleDrive
{
    [ActionList]
    public class Actions
    {
        [Action("Get all items details", Description = "Get all items(files/folders) details")]
        public GetAllItemsResponse GetAllItemsDetails(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] GetAllItemsRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
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
        public GetFileResponse GetFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] GetFileRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
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
        public void UploadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] UploadFileRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
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
        public void DeleteItem(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] DeleteItemRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            client.Files.Delete(input.ItemId).Execute();
        }

        [Action("Create folder", Description = "Create folder")]
        public void CreateFolder(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] CreateFolderRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = input.FolderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { input.ParentFolderId }
            };
            var request = client.Files.Create(fileMetadata);
            request.Execute();
        }

        [Action("Get folder changes by token", Description = "Get folder changes by state token")]
        public GetChangesByTokenResponse GetChangesByToken(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] string token, [ActionParameter] string folderId)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);

            var requestChanges = client.Changes.List(token);
            requestChanges.Spaces = "drive";
            requestChanges.Fields = "*";
            var changes = requestChanges.Execute().Changes.Where(ch => ch.File.Parents.Contains(folderId)).ToList();
            var lastChange = changes.OrderBy(x => x.File.CreatedTime).ToList().Last();
            return new GetChangesByTokenResponse()
            {
                ResourceId = lastChange.File.Id
            };
        }
    }
}
