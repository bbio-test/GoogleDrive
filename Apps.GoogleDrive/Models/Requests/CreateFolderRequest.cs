using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Requests
{
    public class CreateFolderRequest
    {
        [Display("Folder name")]
        public string FolderName { get; set; }

        [Display("Parent folder ID")]
        public string ParentFolderId { get; set; }
    }
}
