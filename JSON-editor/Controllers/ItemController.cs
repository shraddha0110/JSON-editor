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
    public class ItemController : Controller
    {
        private IHostingEnvironment _environment;

        public ItemController(IHostingEnvironment environment)
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

        // GET: Item/Create
        public IActionResult Create(int EventId, int AgendaId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;
            return View();
        }

        // POST: Item/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, int AgendaId, [Bind("ItemId,ItemStart,ItemEnd,ItemTitle,ItemDescription")] Item @item)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();

            if (agenda.Items.LastOrDefault() == null)
            {
                @item.ItemId = 0;
            }
            else
            {
                @item.ItemId = agenda.Items.LastOrDefault().ItemId + 1;
            }

            @item.Slots = new List<Slot>();
            eventlist.Remove(@event);
            agenda.Items.Add(@item);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Item/Edit/5
        public IActionResult Edit(int EventId, int AgendaId, int ItemId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;

            var eventlist = GetList();

            var item = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First();
            return View(item);
        }

        // POST: Item/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int AgendaId, int ItemId, [Bind("ItemId,ItemStart,ItemEnd,ItemTitle,ItemDescription")] Item @item)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item2 = agenda.Items.Where(i => i.ItemId == ItemId).First();
            @item.Slots = item2.Slots;
            eventlist.Remove(@event);
            agenda.Items.Remove(item2);
            agenda.Items.Add(@item);
            agenda.Items = agenda.Items.OrderBy(i => i.ItemId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Item/Delete/5
        public IActionResult Delete(int EventId, int AgendaId, int ItemId)
        {
            ViewBag.EventId = EventId;
            ViewBag.AgendaId = AgendaId;

            var eventlist = GetList();

            var item = eventlist.Where(e => e.EventId == EventId).First()
                .Agendas.Where(a => a.AgendaId == AgendaId).First()
                .Items.Where(i => i.ItemId == ItemId).First();
            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int AgendaId, int ItemId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var agenda = @event.Agendas.Where(a => a.AgendaId == AgendaId).First();
            var item = agenda.Items.Where(i => i.ItemId == ItemId).First();

            eventlist.Remove(@event);
            agenda.Items.Remove(item);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}