namespace Apps.GoogleDrive.Webhooks.Payload
{
    public class FolderContentChangedPayload
    {
        public string StateToken { get; set; }

        public string ResourceState { get; set; } //update, trash

        public string ResourceId { get; set; }

        public string ChannelId { get; set; }
    }
}
