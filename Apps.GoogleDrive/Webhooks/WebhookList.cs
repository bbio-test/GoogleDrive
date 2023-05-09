using Apps.GoogleDrive.Webhooks.Handlers;
using Apps.GoogleDrive.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Google.Apis.Drive.v3.Data;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Apps.GoogleDrive.Webhooks
{
    [WebhookList]
    public class WebhookList 
    {
        [Webhook("On folder content changed", typeof(FolderContentChangedHandler), Description = "On folder conten changed")]
        public async Task<WebhookResponse<FolderContentChangedPayload>> FolderContentChanged(WebhookRequest webhookRequest)
        {
            string stateToken = "";
            string resourceState = "";
            string resourceId = "";
            string channelId = "";
            webhookRequest.Headers.TryGetValue("X-Goog-Channel-Token", out stateToken);
            webhookRequest.Headers.TryGetValue("X-Goog-Resource-State", out resourceState);
            webhookRequest.Headers.TryGetValue("X-Goog-Channel-ID", out channelId);
            webhookRequest.Headers.TryGetValue("X-Goog-Resource-ID", out resourceId);

            return new WebhookResponse<FolderContentChangedPayload>
            {
                HttpResponseMessage = null,
                Result = new FolderContentChangedPayload()
                {
                    StateToken = stateToken,
                    ResourceState = resourceState, //sync, add, remove, update, trash, untrash, change,
                    ChannelId = channelId,
                    ResourceId = resourceId
                }
            };
        }
    }
}
