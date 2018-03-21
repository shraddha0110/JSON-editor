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
    public class SlotDocumentController : Controller
    {
        private IHostingEnvironment _environment;

        public SlotDocumentController(IHostingEnvironment environment)
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

        // GET: SlotDocument/Create
        public IActionResult Create(int EventId, int AgendaId, int ItemId, int SlotId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;
            ViewBag.SlotId = SlotId;
            return View();
        }

        // POST: SlotDocument/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, int AgendaId, int ItemId, int SlotId, [Bind("SlotDocumentId,Title,URL,Type")] SlotDocument @SlotDocument)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();
            var slot = item.Slots.Where(s => s.SlotId == SlotId).First();

            if (slot.SlotDocuments.LastOrDefault() == null)
            {
                @SlotDocument.SlotDocumentId = 0;
            }
            else
            {
                @SlotDocument.SlotDocumentId = slot.SlotDocuments.LastOrDefault().SlotDocumentId + 1;
            }

            eventlist.Remove(@event);
            slot.SlotDocuments.Add(@SlotDocument);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: SlotDocument/Edit/5
        public IActionResult Edit(int EventId, int AgendaId, int ItemId, int SlotId, int SlotDocumentId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;
            ViewBag.SlotId = SlotId;

            var eventlist = GetList();

            var slotDoc = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First()
                .Slots.Where(s => s.SlotId == SlotId).First()
                .SlotDocuments.Where(sd => sd.SlotDocumentId == SlotDocumentId).First();
            return View(slotDoc);
        }

        // POST: SlotDocument/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int AgendaId, int ItemId, int SlotId, int SlotDocumentId, [Bind("SlotDocumentId,Title,URL,Type")] SlotDocument @SlotDocument)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();
            var slot = item.Slots.Where(s => s.SlotId == SlotId).First();
            var slotDoc2 = slot.SlotDocuments.Where(sd => sd.SlotDocumentId == SlotDocumentId).First();

            eventlist.Remove(@event);
            slot.SlotDocuments.Remove(slotDoc2);
            slot.SlotDocuments.Add(@SlotDocument);
            slot.SlotDocuments = slot.SlotDocuments.OrderBy(sd => sd.SlotDocumentId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: SlotDocument/Delete/5
        public IActionResult Delete(int EventId, int AgendaId, int ItemId, int SlotId, int SlotDocumentId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            ViewBag.ItemId = ItemId;
            ViewBag.SlotId = SlotId;

            var eventlist = GetList();

            var slotDoc = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First()
                .Slots.Where(s => s.SlotId == SlotId).First()
                .SlotDocuments.Where(sd => sd.SlotDocumentId == SlotDocumentId).First();
            return View(slotDoc);
        }

        // POST: SlotDocument/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int AgendaId, int ItemId, int SlotId, int SlotDocumentId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();
            var slot = item.Slots.Where(s => s.SlotId == SlotId).First();
            var slotDoc = slot.SlotDocuments.Where(sd => sd.SlotDocumentId == SlotDocumentId).First();

            eventlist.Remove(@event);
            slot.SlotDocuments.Remove(slotDoc);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}