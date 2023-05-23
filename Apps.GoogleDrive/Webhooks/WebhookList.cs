using Apps.GoogleDrive.Webhooks.Handlers;
using Apps.GoogleDrive.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Plugins.Plunet;
using Google.Apis.Drive.v3.Data;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net;

namespace Apps.GoogleDrive.Webhooks
{
    [WebhookList]
    public class WebhookList 
    {
        [Webhook("On folder content added", typeof(FolderContentAddedHandler), Description = "On folder content added")]
        public async Task<WebhookResponse<FolderContentChangedPayload>> FolderContentAdded(WebhookRequest webhookRequest)
        {
            string stateToken = "";
            string resourceState = "";
            string resourceId = "";
            string channelId = "";
            webhookRequest.Headers.TryGetValue("x-goog-channel-token", out stateToken);
            webhookRequest.Headers.TryGetValue("x-goog-resource-state", out resourceState);
            webhookRequest.Headers.TryGetValue("x-goog-channel-id", out channelId);
            webhookRequest.Headers.TryGetValue("x-goog-resource-id", out resourceId);
            if (resourceState != "update")
            {
                return new WebhookResponse<FolderContentChangedPayload>
                {
                    HttpResponseMessage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK },
                    Result = null
                };
            }

            return new WebhookResponse<FolderContentChangedPayload>
            {
                HttpResponseMessage = null,
                Result = new FolderContentChangedPayload()
                {
                    StateToken = stateToken,
                    ResourceState = resourceState, //update, trash
                    ChannelId = channelId,
                    ResourceId = resourceId
                }
            };
        }

        [Webhook("On folder content removed", typeof(FolderContentAddedHandler), Description = "On folder content removed")]
        public async Task<WebhookResponse<FolderContentChangedPayload>> FolderContentRemoved(WebhookRequest webhookRequest)
        {
            string stateToken = "";
            string resourceState = "";
            string resourceId = "";
            string channelId = "";
            webhookRequest.Headers.TryGetValue("x-goog-channel-token", out stateToken);
            webhookRequest.Headers.TryGetValue("x-goog-resource-state", out resourceState);
            webhookRequest.Headers.TryGetValue("x-goog-channel-id", out channelId);
            webhookRequest.Headers.TryGetValue("x-goog-resource-id", out resourceId);
            if (resourceState != "trash")
            {
                return new WebhookResponse<FolderContentChangedPayload>
                {
                    HttpResponseMessage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK },
                };
            }
            return new WebhookResponse<FolderContentChangedPayload>
            {
                HttpResponseMessage = null,
                Result = new FolderContentChangedPayload()
                {
                    StateToken = stateToken,
                    ResourceState = resourceState, //update, trash
                    ChannelId = channelId,
                    ResourceId = resourceId
                }
            };
        }
    }
}
