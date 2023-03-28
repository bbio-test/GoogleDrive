using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Models.Responses
{
    public class GetFileResponse
    {
        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}
