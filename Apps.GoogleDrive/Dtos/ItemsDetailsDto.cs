using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Dtos
{
    public class ItemsDetailsDto
    {
        [Display("File ID")]
        public string Id { get; set; }
        public string Name { get; set; }

        [Display("MIME type")]
        public string MimeType { get; set; }
    }
}