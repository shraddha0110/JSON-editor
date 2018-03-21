using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JSONEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace JSONEditor.Controllers
{
	public class HomeController : Controller
	{
		private IHostingEnvironment _environment;

		public HomeController(IHostingEnvironment environment)
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
			string file_name = string.Empty;
			if (Request.Cookies["file_name"] != null)
			{
				file_name = Request.Cookies["file_name"];
			}
			else
			{
				file_name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
				CreateCookie(file_name);
			}
			var uploads = Path.Combine(_environment.WebRootPath, "uploads");
			string jsondata = JsonConvert.SerializeObject(eventlist);
			System.IO.File.WriteAllText($"{uploads}/{file_name}", jsondata);
		}

		public void CreateCookie(string file_name)
		{
			CookieOptions options = new CookieOptions();
			options.Expires = DateTime.Now.AddDays(1);
			Response.Cookies.Append("file_name", file_name, options);
		}

		public IActionResult Index()
		{
			var eventlist = new List<Event>();
			ViewBag.ShowDownloadBtn = "hidden";
			if (Request.Cookies["file_name"] != null)
			{
				eventlist = GetList();

				if (eventlist.Count() > 0)
				{
					ViewBag.ShowDownloadBtn = "visible";
				}
			}
			return View(eventlist);
		}

		[HttpPost]
		public IActionResult Index(ICollection<IFormFile> files)
		{
			string Json = string.Empty;
			string file_name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + System.IO.Path.GetExtension(files.First().FileName);
			var eventlist = new List<Event>();
			var uploads = Path.Combine(_environment.WebRootPath, "uploads");
			ViewBag.file_name = file_name;

			foreach (var file in files)
			{
				if (file != null && file.Length > 0)
				{
					using (var fileStream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create))
					{
						file.CopyTo(fileStream);

						using (var reader = new StreamReader(file.OpenReadStream()))
						{
							Json = reader.ReadToEnd();
						}
					}
				}
			}

			eventlist = JsonConvert.DeserializeObject<List<Event>>(Json);

			CreateCookie(file_name);

			return View(eventlist);
		}

		public IActionResult CreateNew()
		{
			string file_name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
			CreateCookie(file_name);

			return RedirectToAction("Index");
		}

		public FileResult DownloadFile()
		{
			var file_name = Request.Cookies["file_name"];
			var filepath = Path.Combine(_environment.WebRootPath, "uploads", file_name);
			byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
			return File(fileBytes, "application/json", file_name);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create([Bind("EventId,Type,Title,Start,End,CountryCode,CountryName,City,Venue,Description,Image")] Event @event)
		{
			var eventlist = GetList();

			if (eventlist.LastOrDefault() == null)
			{
				@event.EventId = 0;
			}
			else
			{
				@event.EventId = eventlist.LastOrDefault().EventId + 1;
			}

			@event.Contacts = new List<Contact>();
			@event.Topics = new List<Topic>();
			@event.EventDocuments = new List<EventDocument>();
			@event.Agendas = new List<Agenda>();
			eventlist.Add(@event);

			SetList(eventlist);
			return RedirectToAction("Index");
		}

		public IActionResult Edit(int id)
		{
			var eventlist = GetList();
			var @event = eventlist.Where(e => e.EventId == id);
			return View(@event.First());
		}

		[HttpPost]
		public IActionResult Edit(int id, [Bind("EventId,Type,Title,Start,End,CountryCode,CountryName,City,Venue,Description,Image")] Event @event)
		{
			var eventlist = GetList();
			var event2 = eventlist.Where(e => e.EventId == id).First();

			@event.Contacts = event2.Contacts;
			@event.Topics = event2.Topics;
			@event.EventDocuments = event2.EventDocuments;
			@event.Agendas = event2.Agendas;

			eventlist.Remove(event2);
			eventlist.Add(@event);
			eventlist = eventlist.OrderBy(e => e.EventId).ToList();

			SetList(eventlist);
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var eventlist = GetList();
			var @event = eventlist.Where(e => e.EventId == id);
			return View(@event.First());
		}


		[HttpPost, ActionName("Delete")]
		public IActionResult DeleteConfirmed(int id)
		{
			var eventlist = GetList();
			var event2 = eventlist.Where(e => e.EventId == id);
			eventlist.Remove(event2.First());
			SetList(eventlist);
			return RedirectToAction("Index");
		}

		public IActionResult Details()
		{
			return View();
		}
	}
}
