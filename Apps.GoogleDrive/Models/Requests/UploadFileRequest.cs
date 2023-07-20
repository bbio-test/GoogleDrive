using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Requests
{
    public class UploadFileRequest
    {
        public string Name { get; set; }

        public byte[] File { get; set; }

        [Display("Parent folder ID")]
        public string ParentFolderId { get; set; }
    }
}
