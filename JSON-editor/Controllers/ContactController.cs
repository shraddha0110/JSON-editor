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
    public class ContactController : Controller
    {
        private IHostingEnvironment _environment;

        public ContactController(IHostingEnvironment environment)
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

        // GET: Contact
        public IActionResult Index()
        {
            return View();
        }

        // GET: Contact/Create
        public IActionResult Create(int EventId)
        {
            ViewBag.EventId = EventId;
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, [Bind("ContactId,Name,Phone,Email")] Contact @contact)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            if (@event.Contacts.LastOrDefault() == null)
            {
                @contact.ContactId = 0;
            }
            else
            {
                @contact.ContactId = @event.Contacts.LastOrDefault().ContactId + 1;
            }

            eventlist.Remove(@event);
            @event.Contacts.Add(@contact);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }


        // GET: Contact/Edit/5
        public IActionResult Edit(int EventId, int ContactId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var contact = eventlist.Where(e => e.EventId == EventId).First()
                .Contacts.Where(c => c.ContactId == ContactId).First();
            return View(contact);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int ContactId, [Bind("ContactId,Name,Phone,Email")] Contact @contact)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var contact2 = @event.Contacts.Where(c => c.ContactId == ContactId).First();
            eventlist.Remove(@event);
            @event.Contacts.Remove(contact2);
            @event.Contacts.Add(@contact);
            @event.Contacts = @event.Contacts.OrderBy(c => c.ContactId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Contact/Delete/5
        public IActionResult Delete(int EventId, int ContactId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var contact = eventlist.Where(e => e.EventId == EventId).First()
                .Contacts.Where(c => c.ContactId == ContactId).First();
            return View(contact);
        }

        //POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int ContactId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var contact = @event.Contacts.Where(c => c.ContactId == ContactId).First();

            eventlist.Remove(@event);
            @event.Contacts.Remove(contact);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}
