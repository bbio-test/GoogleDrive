using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Webhooks.Payload
{
    public class FolderContentChangedPayload
    {
        public string StateToken { get; set; }

        public string ResourceState { get; set; }

        public string ResourceId { get; set; }

        public string ChannelId { get; set; }
    }
}
