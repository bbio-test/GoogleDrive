using Apps.GoogleDrive.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.GoogleDrive.Models.Responses
{
    public class GetAllItemsResponse
    {
        [Display("Items details")]
        public IEnumerable<ItemsDetailsDto> ItemsDetails { get; set; }
    }
}
