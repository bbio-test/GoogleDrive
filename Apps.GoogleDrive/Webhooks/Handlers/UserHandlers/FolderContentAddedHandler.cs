using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.GoogleDrive.Webhooks.Handlers.UserHandlers;

public class FolderContentAddedHandler : BaseWebhookHandler
{
    public FolderContentAddedHandler([WebhookParameter] WebhookInput input) : base(input) { }
}