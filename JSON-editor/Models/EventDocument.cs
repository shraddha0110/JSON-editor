using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONEditor.Models
{
    public class EventDocument
    {
        public int EventDocumentId { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
    }
}
