using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Requests;

public class GetChangedFilesRequest
{
    [Display("Last hours")]
    public int LastHours { get; set; }
}