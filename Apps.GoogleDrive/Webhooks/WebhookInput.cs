using Apps.GoogleDrive.DataSourceHandler;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.GoogleDrive.Webhooks;

public class WebhookInput
{
    [Display("Resource ID")]
    [DataSource(typeof(FolderDataHandler))]
    public string ResourceId { get; set; }
}