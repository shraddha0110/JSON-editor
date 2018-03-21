using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JSONEditor.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace JSONEditor.Controllers
{
    public class SlotController : Controller
    {
        private IHostingEnvironment _environment;

        public SlotController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public List<Event> GetList()
        {
            var eventlist = new List<Event>();
            try
            {
                var file_name = Request.Cookies["file_name"];
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                var Json = System.IO.File.ReadAllText($"{uploads}/{file_name}");
                eventlist = JsonConvert.DeserializeObject<List<Event>>(Json);
            }
            catch
            {
                //TODO (Display Warning)
            }

            return eventlist;
        }

        public void SetList(List<Event> eventlist)
        {
            var file_name = Request.Cookies["file_name"];
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            string jsondata = JsonConvert.SerializeObject(eventlist);
            System.IO.File.WriteAllText($"{uploads}/{file_name}", jsondata);
        }

        // GET: Slot/Create
        public IActionResult Create(int EventId, int AgendaId, int ItemId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;
            return View();
        }

        // POST: Slot/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, int AgendaId, int ItemId, [Bind("SlotId,SpeakerName,SpeakerPosition,Organization,Country,Title,Description")] Slot @slot)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();

            if (item.Slots.LastOrDefault() == null)
            {
                @slot.SlotId = 0;
            }
            else
            {
                @slot.SlotId = item.Slots.LastOrDefault().SlotId + 1;
            }

            @slot.SlotDocuments = new List<SlotDocument>();
            eventlist.Remove(@event);
            item.Slots.Add(@slot);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Slot/Edit/5
        public IActionResult Edit(int EventId, int AgendaId, int ItemId, int SlotId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;

            var eventlist = GetList();

            var slot = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First()
                .Slots.Where(s => s.SlotId == SlotId).First();
            return View(slot);
        }

        // POST: Slot/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int AgendaId, int ItemId, int SlotId, [Bind("SlotId,SpeakerName,SpeakerPosition,Organization,Country,Title,Description")] Slot @slot)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();
            var slot2 = item.Slots.Where(s => s.SlotId == SlotId).First();

            @slot.SlotDocuments = slot2.SlotDocuments;
            eventlist.Remove(@event);
            item.Slots.Remove(slot2);
            item.Slots.Add(@slot);
            item.Slots = item.Slots.OrderBy(s => s.SlotId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Slot/Delete/5
        public IActionResult Delete(int EventId, int AgendaId, int ItemId, int SlotId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;

            var eventlist = GetList();

            var slot = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First()
                .Slots.Where(s => s.SlotId == SlotId).First();
            return View(slot);
        }

        // POST: Slot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int AgendaId, int ItemId, int SlotId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();
            var slot = item.Slots.Where(s => s.SlotId == SlotId).First();

            eventlist.Remove(@event);
            item.Slots.Remove(slot);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}