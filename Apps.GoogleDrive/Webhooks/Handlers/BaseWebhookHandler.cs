using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Google.Apis.Drive.v3.Data;

namespace Apps.GoogleDrive.Webhooks.Handlers
{
    public class BaseWebhookHandler : IWebhookEventHandler
    {
        public BaseWebhookHandler()
        {
        }

        public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
        {
            var client = new GoogleDriveClient(authenticationCredentialsProvider);
            var stateToken = client.Changes.GetStartPageToken().Execute();
            var channnelId = Guid.NewGuid().ToString();
            var request = client.Files.Watch(new Channel()
            {
                Id = channnelId,
                Type = "web_hook",
                Address = values["payloadUrl"],
                Token = stateToken.StartPageTokenValue,
                Payload = true
            }, values["resourceIdForWebhook"]);
            request.Execute();
        }

        public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
        {
            await Task.CompletedTask;
        }
    }
}
