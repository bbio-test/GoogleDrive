using Apps.GoogleDrive.Dtos;
using Apps.GoogleDrive.Models.Requests;
using Apps.GoogleDrive.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Actions
{
    [ActionList]
    public class ChangesActions
    {
        [Action("Get changed files", Description = "Get all files that have been created or modified")]
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
    }
}
