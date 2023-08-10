using Apps.GoogleDrive.DataSourceHandler;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleDrive.Models.Requests
{
    public class UploadFileRequest
    {
        public string Name { get; set; }

        public byte[] File { get; set; }

        [Display("Parent folder")]
        [DataSource(typeof(FolderDataHandler))]
        public string ParentFolderId { get; set; }
    }
}
