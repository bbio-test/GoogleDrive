using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Webhooks
{
    public class WebhookInput
    {
        [Display("Resource ID")]
        public string ResourceId { get; set; }
    }
}
