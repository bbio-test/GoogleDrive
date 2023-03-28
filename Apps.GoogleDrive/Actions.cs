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

namespace Apps.GoogleDrive
{
    [ActionList]
    public class Actions
    {
        [Action("Get all items details", Description = "Get all items(files/folders) details")]
        public GetAllItemsResponse GetAllFilesDetails(AuthenticationCredentialsProvider authenticationCredentialsProvider,
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
                FilesDetails = filesDetails
            };
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
