using Apps.GoogleDrive.Clients;
using Apps.GoogleDrive.Dtos;
using Apps.GoogleDrive.Models.Requests;
using Apps.GoogleDrive.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.Drive.v3.Data;

namespace Apps.GoogleDrive.Actions
{
    [ActionList]
    public class StorageActions
    {
        #region File actions

        [Action("Get all items details", Description = "Get all items(files/folders) details")]
        public GetAllItemsResponse GetAllItemsDetails(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            var filesListr = client.Files.List();
            filesListr.SupportsAllDrives = true;
            var filesList = filesListr.Execute();
            var filesDetails = new List<ItemsDetailsDto>();
            foreach (var file in filesList.Files)
            {
                filesDetails.Add(new ItemsDetailsDto()
                {
                    Id = file.Id,
                    Name = file.Name,
                    Type = file.MimeType.Equals("application/vnd.google-apps.folder") ? "folder" : "file"
                });
            }

            return new GetAllItemsResponse()
            {
                ItemsDetails = filesDetails
            };
        }

        [Action("Get changed files", Description = "Get all files that have been created or modified")]
        public async Task<GetAllItemsResponse> GetChangedFiles(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetChangedFilesRequest input)
        {
            var client = new GoogleDriveActivityClient(authenticationCredentialsProviders);
            var fileIds = new List<string?>();

            string? pageToken = null;
            var filterTime = (DateTimeOffset)(DateTime.Now - TimeSpan.FromHours(input.LastHours));
    
            do
            {
                var request = client.Activity.Query(new()
                {
                    Filter = $"time >= {filterTime.ToUnixTimeMilliseconds()}",
                    PageToken = pageToken
                });

                var response = await request.ExecuteAsync();
                pageToken = response.NextPageToken;

                var filteredIds = response.Activities
                    .Select(x => x.Targets?.FirstOrDefault()?.DriveItem?.Name.Split("/").Last());
                fileIds.AddRange(filteredIds);
            } while (!string.IsNullOrEmpty(pageToken));
       
            var allFiles = GetAllItemsDetails(authenticationCredentialsProviders);

            return new()
            {
                ItemsDetails = allFiles.ItemsDetails
                    .Where(x => fileIds.Any(y => y == x.Id))
            };
        }
        
        [Action("Get file", Description = "Get file by Id")]
        public GetFileResponse GetFile(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
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

        #endregion

        #region Folder actions

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

        [Action("Get folder change by token", Description = "Get last folder change by state token")]
        public GetChangesByTokenResponse GetChangesByToken(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string stateToken, [ActionParameter] string folderId,
            [ActionParameter] string resourceState)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            var lastChange = new Change();

            var requestChanges = client.Changes.List(stateToken);
            requestChanges.Spaces = "drive";
            requestChanges.Fields = "*";
            if (resourceState == "update")
            {
                var changes = requestChanges.Execute().Changes
                    .Where(ch => ch.File.Parents != null && ch.File.Parents.Contains(folderId)).ToList();
                lastChange = changes.OrderBy(x => x.File.CreatedTime).ToList().Last();
            }
            else if (resourceState == "trash")
            {
                var changes = requestChanges.Execute().Changes.Where(ch => ch.File.Trashed ?? false).ToList();
                lastChange = changes.OrderBy(x => x.File.TrashedTime).ToList().Last();
            }

            return new GetChangesByTokenResponse()
            {
                ResourceId = lastChange.File.Id
            };
        }

        #endregion
    }
}