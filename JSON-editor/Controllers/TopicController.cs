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
    public class TopicController : Controller
    {
        private IHostingEnvironment _environment;

        public TopicController(IHostingEnvironment environment)
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

        // GET: Topic
        public IActionResult Index()
        {
            return View();
        }

        // GET: Topic/Create
        public IActionResult Create(int EventId)
        {
            ViewBag.EventId = EventId;
            return View();
        }

        // POST: Topic/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int EventId, [Bind("TopicId,Content")] Topic @topic)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            if (@event.Topics.LastOrDefault() == null)
            {
                @topic.TopicId = 0;
            }
            else
            {
                @topic.TopicId = @event.Topics.LastOrDefault().TopicId + 1;
            }

            eventlist.Remove(@event);
            @event.Topics.Add(@topic);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }


        // GET: Topic/Edit/5
        public IActionResult Edit(int EventId, int TopicId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var topic = eventlist.Where(e => e.EventId == EventId).First()
                .Topics.Where(t => t.TopicId == TopicId).First();
            return View(topic);
        }

        // POST: Topic/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int EventId, int TopicId, [Bind("TopicId,Content")] Topic @topic)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var topic2 = @event.Topics.Where(t => t.TopicId == TopicId).First();
            eventlist.Remove(@event);
            @event.Topics.Remove(topic2);
            @event.Topics.Add(@topic);
            @event.Topics = @event.Topics.OrderBy(t => t.TopicId).ToList();
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }

        // GET: Topic/Delete/5
        public IActionResult Delete(int EventId, int TopicId)
        {
            ViewBag.EventId = EventId;
            var eventlist = GetList();

            var topic = eventlist.Where(e => e.EventId == EventId).First()
                .Topics.Where(t => t.TopicId == TopicId).First();
            return View(topic);
        }

        //POST: Topic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int EventId, int TopicId)
        {
            var eventlist = GetList();

            var @event = eventlist.Where(e => e.EventId == EventId).First();
            var topic = @event.Topics.Where(t => t.TopicId == TopicId).First();

            eventlist.Remove(@event);
            @event.Topics.Remove(topic);
            eventlist.Add(@event);
            eventlist = eventlist.OrderBy(e => e.EventId).ToList();

            SetList(eventlist);
            return RedirectToAction("Index", "Home");
        }
    }
}
