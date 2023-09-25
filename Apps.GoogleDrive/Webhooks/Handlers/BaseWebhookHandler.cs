using Apps.GoogleDrive.Clients;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Google.Apis.Drive.v3.Data;

namespace Apps.GoogleDrive.Webhooks.Handlers;

public class BaseWebhookHandler : IWebhookEventHandler, IAsyncRenewableWebhookEventHandler
{
    private string ResourceId { get; }

    public BaseWebhookHandler([WebhookParameter] WebhookInput input)
    {
        ResourceId = input.ResourceId;
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var client = new GoogleDriveClient(creds);

        var stateToken = client.Changes.GetStartPageToken().Execute();
        var channnelId = Guid.NewGuid().ToString();
        var request = client.Files.Watch(new Channel
        {
            Id = channnelId,
            Type = "web_hook",
            Address = values["payloadUrl"],
            Token = stateToken.StartPageTokenValue,
            Payload = true,
        }, ResourceId);

        await request.ExecuteAsync();
    }

    public Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
        => Task.CompletedTask;

    [Period(60)]
    public Task RenewSubscription(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
        => SubscribeAsync(creds, values);
}