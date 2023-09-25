using Apps.GoogleDrive.DataSourceHandler;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.GoogleDrive.Models.Requests;

public class UploadFileRequest
{
    public File File { get; set; }

    [Display("Parent folder")]
    [DataSource(typeof(FolderDataHandler))]
    public string ParentFolderId { get; set; }
}