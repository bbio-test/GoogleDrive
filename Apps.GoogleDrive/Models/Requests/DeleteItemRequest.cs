using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Requests
{
    public class DeleteItemRequest
    {
        [Display("Item ID")]
        public string ItemId { get; set; }
    }
}
