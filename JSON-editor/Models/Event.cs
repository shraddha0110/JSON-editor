using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace JSONEditor.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }

        public string Description { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Topic> Topics { get; set; }

        public string Image { get; set; }

        public List<EventDocument> EventDocuments { get; set; }

        public List<Agenda> Agendas { get; set; }
    }
}