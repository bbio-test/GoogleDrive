namespace Apps.GoogleDrive.Models.Requests
{
    public class UploadFileRequest
    {
        public string Filename { get; set; }

        public byte[] File { get; set; }

        public string ParentFolderId { get; set; }
    }
}
