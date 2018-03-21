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
    public class AgendaController : Controller
    {
        private IHostingEnvironment _environment;

        public AgendaController(IHostingEnvironment environment)
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

        // GET: Agenda/Create
        public ActionResult Create(int EventId)
        {
            ViewBag.EventId = EventId;
            return View();
        }

        // POST: Agenda/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, [Bind("Date")] Agenda @agenda)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();

            if (@event.Agendas.LastOrDefault() == null)
            {
                @agenda.AgendaId = 0;
            }
            else
            {
                @agenda.AgendaId = @event.Agendas.LastOrDefault().AgendaId + 1;
            }

            @agenda.Items = new List<Item>();
            eventlist.Remove(@event);
            @event.Agendas.Add(@agenda);
            eventlist.Add(@event);

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Agenda/Edit/5
        public IActionResult Edit(int EventId, int AgendaId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var agenda = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First();
            return View(agenda);
        }

        // POST: Agenda/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int AgendaId, [Bind("AgendaId,Date")] Agenda @agenda)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda2 = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            @agenda.Items = agenda2.Items;
            eventlist.Remove(@event);
            @event.Agendas.Remove(agenda2);
            @event.Agendas.Add(@agenda);
            @event.Agendas = @event.Agendas.OrderBy(a => a.AgendaId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Agenda/Delete/5
        public IActionResult Delete(int EventId, int AgendaId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var agenda = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First();
            return View(agenda);
        }

        // POST: Agenda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int AgendaId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();

            eventlist.Remove(@event);
            @event.Agendas.Remove(agenda);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}