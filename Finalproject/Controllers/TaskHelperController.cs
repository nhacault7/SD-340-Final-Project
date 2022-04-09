using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Finalproject.Controllers
{
    public class TaskHelperController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public TaskHelperController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // GET: TaskHelperController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TaskHelperController/Details/5
        public ActionResult Details(int taskId)
        {
            try
            {
                ProjectTask currTask = _db.Tasks.First(t => t.Id == taskId);
                if ( currTask != null )
                {
                    return View(currTask);
                }
                else
                {
                    return NotFound();
                }
            }
            catch ( Exception ex )
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: TaskHelperController/Create
        public ActionResult Create(int projectId)
        {
            ViewBag.projectId = projectId;
            List<SelectListItem> priorities = new List<SelectListItem>
            {
                new SelectListItem(){ Text = "Urgent", Value = Priority.Urgent.ToString() },
                new SelectListItem(){ Text = "High", Value = Priority.High.ToString() },
                new SelectListItem(){ Text = "Medium", Value = Priority.Medium.ToString() },
                new SelectListItem(){ Text = "Low", Value = Priority.Low.ToString() }
            };
            ViewBag.priorityList = priorities;
            return View();
        }

        // POST: TaskHelperController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    ProjectTask newTask = new ProjectTask();
                    newTask.Name = collection["Name"].ToString();
                    newTask.Priority = (Priority)Enum.Parse(typeof(Priority), collection["Priority"].ToString());
                    newTask.DeadLine = DateTime.Parse(collection["Deadline"]);
                    newTask.ProjectId = int.Parse(collection["ProjectId"].ToString());

                    //default value for a new created task
                    newTask.PercentageCompleted = 0;
                    newTask.StartDate = DateTime.Today;
                    newTask.IsCompleted = false;
                    _db.Tasks.Add(newTask);
                    _db.SaveChanges();

                    return RedirectToAction("Details", "ProjectHelper", new
                    {
                        projectId = newTask.ProjectId
                    });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskHelperController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskHelperController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskHelperController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskHelperController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
