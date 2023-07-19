using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Requests
{
    public class GetFileRequest
    {
        [Display("File ID")]
        public string FileId { get; set; }
    }
}
