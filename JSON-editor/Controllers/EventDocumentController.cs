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
    public class EventDocumentController : Controller
    {
        private IHostingEnvironment _environment;

        public EventDocumentController(IHostingEnvironment environment)
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

        // GET: EventDocument
        public IActionResult Index()
        {
            return View();
        }

        // GET: EventDocument/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: EventDocument/Create
        public IActionResult Create(int EventId)
        {
            ViewBag.EventId = EventId;
            return View();
        }

        // POST: EventDocument/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, [Bind("EventDocumentId,Title,URL,Type")] EventDocument @EventDocument)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            if (@event.EventDocuments.LastOrDefault() == null)
            {
                @EventDocument.EventDocumentId = 0;
            }
            else
            {
                @EventDocument.EventDocumentId = @event.EventDocuments.LastOrDefault().EventDocumentId + 1;
            }

            eventlist.Remove(@event);
            @event.EventDocuments.Add(@EventDocument);
            @event.EventDocuments = @event.EventDocuments.OrderBy(ed => ed.EventDocumentId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: EventDocument/Edit/5
        public IActionResult Edit(int EventDocumentId, int EventId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var EventDoc = eventlist.Where(e => e.EventId == EventId).First()
               .EventDocuments.Where(ed => ed.EventDocumentId == EventDocumentId).First();
            return View(EventDoc);
        }

        // POST: EventDocument/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int EventDocumentId, [Bind("EventDocumentId,Title,URL,Type")] EventDocument @EventDocument)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var EventDoc = @event.EventDocuments.Where(ed => ed.EventDocumentId == EventDocumentId).First();
            eventlist.Remove(@event);
            @event.EventDocuments.Remove(EventDoc);
            @event.EventDocuments.Add(@EventDocument);
            @event.EventDocuments = @event.EventDocuments.OrderBy(ed => ed.EventDocumentId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();
            
            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: EventDocument/Delete/5
        public IActionResult Delete(int EventId, int EventDocumentId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var EventDoc = eventlist.Where(e => e.EventId == EventId).First()
               .EventDocuments.Where(ed => ed.EventDocumentId == EventDocumentId).First();
            return View(EventDoc);
        }

        // POST: EventDocument/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int EventDocumentId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var EventDoc = @event.EventDocuments.Where(ed => ed.EventDocumentId == EventDocumentId).First();
            eventlist.Remove(@event);
            @event.EventDocuments.Remove(EventDoc);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}