using Apps.GoogleDrive.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Models.Responses
{
    public class GetAllItemsResponse
    {
        public IEnumerable<ItemsDetailsDto> FilesDetails { get; set; }
    }
}
