using Apps.GoogleDrive.Clients;
using Apps.GoogleDrive.Dtos;
using Apps.GoogleDrive.Models.Requests;
using Apps.GoogleDrive.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.Download;
using Google.Apis.DriveActivity.v2.Data;

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
                filesDetails.Add(new ItemsDetailsDto
                {
                    Id = file.Id,
                    Name = file.Name,
                    MimeType = file.MimeType
                });
            }

            return new GetAllItemsResponse(filesDetails);
        }

        [Action("Get changed files", Description = "Get all files that have been created or modified in the last time period")]
        public async Task<GetChangedItemsResponse> GetChangedFiles(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetChangedFilesRequest input)
        {
            var activityClient = new GoogleDriveActivityClient(authenticationCredentialsProviders);
            var driveItems = new List<DriveItem>();
            var deletedItemIds = new List<string>();

            string? pageToken = null;
            var filterTime = (DateTimeOffset)(DateTime.Now - TimeSpan.FromHours(input.LastHours));
    
            do
            {
                var request = activityClient.Activity.Query(new()
                {
                    Filter = $"time >= {filterTime.ToUnixTimeMilliseconds()} AND detail.action_detail_case:(CREATE EDIT DELETE)",
                    PageToken = pageToken
                });

                var response = await request.ExecuteAsync();
                pageToken = response.NextPageToken;

                var deletedItems = response.Activities?
                    .Where(x => x.PrimaryActionDetail.Delete != null)
                    .Select(x => x.Targets?.FirstOrDefault()?.DriveItem)
                    .Where(x => x != null)
                    .Where(x => x.MimeType != "application/vnd.google-apps.folder")
                    .Select(x => x.Name);

                var items = response.Activities?
                    .Where(x => x.PrimaryActionDetail.Create != null || x.PrimaryActionDetail.Edit != null)
                    .Select(x => x.Targets?.FirstOrDefault()?.DriveItem)
                    .Where(x => x != null)
                    .Where(x => x.MimeType != "application/vnd.google-apps.folder");

                if (items != null)
                    driveItems.AddRange(items);

                if (deletedItems != null)
                    deletedItemIds.AddRange(deletedItems);
            } while (!string.IsNullOrEmpty(pageToken));

            var allChangedItems = driveItems.Where(x => !deletedItemIds.Contains(x.Name)).DistinctBy(x => x.Name);

            return new GetChangedItemsResponse
            {
                ItemsDetails = allChangedItems.Select(x => new ItemsDetailsDto 
                { 
                    Name = x.Title, 
                    Id = x.Name.Split("/").Last(),
                    MimeType= x.MimeType,
                })
            };
        }

        private Dictionary<string, string> _mimeMap = new Dictionary<string, string>
        {
            { "application/vnd.google-apps.document", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "application/vnd.google-apps.presentation", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "application/vnd.google-apps.spreadsheet", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "application/vnd.google-apps.drawing", "application/pdf" }
        };

        private Dictionary<string, string> _extensionMap = new Dictionary<string, string>
        {
            { "application/vnd.google-apps.document", ".docx" },
            { "application/vnd.google-apps.presentation", ".pptx" },
            { "application/vnd.google-apps.spreadsheet", ".xlsx" },
            { "application/vnd.google-apps.drawing", ".pdf" }
        };

        [Action("Download file", Description = "Download a file")]
        public GetFileResponse GetFile(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetFileRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            var request = client.Files.Get(input.FileId);
            var fileMetadata = request.Execute();

            byte[] data;
            var fileName = fileMetadata.Name;
            using (var stream = new MemoryStream())
            {
                if (fileMetadata.MimeType.Contains("vnd.google-apps"))
                {
                    if (!_mimeMap.ContainsKey(fileMetadata.MimeType))
                        throw new Exception($"The file {fileMetadata.Name} has type {fileMetadata.MimeType}, which has no defined conversion");
                    var exportRequest = client.Files.Export(input.FileId, _mimeMap[fileMetadata.MimeType]);
                    exportRequest.DownloadWithStatus(stream).ThrowOnFailure();
                    fileName += _extensionMap[fileMetadata.MimeType];
                }
                else                    
                    request.DownloadWithStatus(stream).ThrowOnFailure();

                data = stream.ToArray();
            }

            return new GetFileResponse
            {
                Name = fileName,
                Data = data
            };
        }

        [Action("Upload file", Description = "Upload a file")]
        public void UploadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] UploadFileRequest input)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProviders);
            var body = new Google.Apis.Drive.v3.Data.File();
            body.Name = input.Name;
            body.Parents = new List<string> { input.ParentFolderId };

            using (var stream = new MemoryStream(input.File))
            {
                var request = client.Files.Create(body, stream, null);
                request.Upload();
            }
        }

        [Action("Delete item", Description = "Delete item (file/folder)")]
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
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = input.FolderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { input.ParentFolderId }
            };
            var request = client.Files.Create(fileMetadata);
            request.Execute();
        }

        #endregion
    }
}