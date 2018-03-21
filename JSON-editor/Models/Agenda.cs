using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JSONEditor.Models
{
    public class Agenda
    {
        public int AgendaId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public List<Item> Items { get; set; }
    }
}
