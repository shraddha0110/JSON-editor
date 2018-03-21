using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONEditor.Models
{
    public class Slot
    {
        public int SlotId { get; set; }
        public string SpeakerName { get; set; }
        public string SpeakerPosition { get; set; }
        public string Organization { get; set; }
        public string Country { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<SlotDocument> SlotDocuments { get; set; }
    }
}
