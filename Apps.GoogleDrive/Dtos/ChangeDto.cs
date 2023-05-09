using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive.Dtos
{
    public class ChangeDto
    {
        public string ResourceId { get; set; }

        public IEnumerable<string> ParentFolders { get; set; }
    }
}
