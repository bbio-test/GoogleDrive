using Apps.GoogleDrive.Clients;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Google.Apis.Drive.v3.Data;

namespace Apps.GoogleDrive.Webhooks.Handlers;

public class ChangesHandler : IWebhookEventHandler
{
    private readonly WebhookInput _webhookInput;

    public ChangesHandler([WebhookParameter] WebhookInput input)
    {
        _webhookInput = input;
    }

    private Channel BuildChannel(Dictionary<string, string> values)
    {
        return new Channel
        {
            Payload = true,
            Id = values["payloadUrl"].Split('/').Last(),
            ResourceId = _webhookInput.ResourceId,
            Expiration = new DateTimeOffset(DateTime.Now.AddDays(7)).ToUnixTimeMilliseconds(),
            Type = "web_hook",
            Address = values["payloadUrl"]
        };
    }

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
    {
        var client = new GoogleDriveClient(authenticationCredentialsProvider);
        var channel = BuildChannel(values);
        var stateToken = client.Changes.GetStartPageToken().Execute();
        var request = client.Changes.Watch(channel, stateToken.StartPageTokenValue);
        await request.ExecuteAsync();
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
    {
        var client = new GoogleDriveClient(authenticationCredentialsProvider);
        var channel = BuildChannel(values);
        var request = client.Channels.Stop(channel);
        await request.ExecuteAsync();
    }
}