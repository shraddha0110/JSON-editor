using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONEditor.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemStart { get; set; }
        public string ItemEnd { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }

        public List<Slot> Slots { get; set; }
    }
}
