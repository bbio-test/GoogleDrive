using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Webhooks
{
    public class WebhookInput
    {
        [Display("Resource ID")]
        public string ResourceId { get; set; }
    }
}
