using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.GoogleDrive.Webhooks.Handlers.UserHandlers;

public class FolderContentRemovedHandler : BaseWebhookHandler
{
    public FolderContentRemovedHandler([WebhookParameter] WebhookInput input) : base(input) { }
}