using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DevelopmentController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public DevelopmentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: DevelopmentController
        public async Task<ActionResult> Index(string orderString, string searchString, int? pageNumber)
        {
            //Get all projects only belonged to the current logged in Developer
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var allTasksOfDev = _db.Tasks.Include(t => t.Project).Where(t => t.UserCreator.Id == currentUser.Id);

            if (orderString != null)
            {
                if (orderString == "IsCompleted")
                {
                    allTasksOfDev = allTasksOfDev.OrderByDescending(t => t.IsCompleted);
                }
                else if (orderString == "PercentageCompleted")
                {
                    allTasksOfDev = allTasksOfDev.OrderByDescending(t => t.PercentageCompleted);
                }
                else
                {
                    allTasksOfDev = allTasksOfDev.OrderBy(t => t.Priority);
                }

            }

            //Pagination
            int pageSize = 10;//the max value is 10 records in every page
            //get filter items when search string is not null
            if (searchString != null)
            {
                var searchedTasks = allTasksOfDev.Where(t => t.Name.Contains(searchString));

                //converts the project query to a single page of projects in a collection type that supports paging.
                //The AsNoTracking() extension method returns a new query and the returned entities will not be cached by the context (DbContext or Object Context). 
                return View(await PaginatedList<ProjectTask>.CreateAsync(searchedTasks.AsNoTracking(), pageNumber ?? 1, pageSize)); //if pageNumber is null, pageNumber=1,else pageNumber = pageNumber
            }

            return View(await PaginatedList<ProjectTask>.CreateAsync(allTasksOfDev.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: DevelopmentController/TaskDetails/5
        public ActionResult TaskDetails(int taskId)
        {
            ProjectTask task = _db.Tasks.Include(t => t.Project).Include(t => t.Comments.OrderByDescending(c => c.CommentType)).First(t => t.Id == taskId);
            return View(task);
        }


        // GET: DevelopmentController/UpdateTask/5
        public ActionResult UpdateTask(int taskId)
        {
            ProjectTask task = _db.Tasks.Include(t => t.Project).First(t => t.Id == taskId);
            return View(task);
        }

        // POST: DevelopmentController/UpdateTask/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTask(int id, IFormCollection collection)
        {
            try
            {
                ProjectTask taskToUpdate = _db.Tasks.Find(id);
                if (taskToUpdate == null)
                {
                    return NotFound();
                }
                taskToUpdate.PercentageCompleted = double.Parse(collection["PercentageCompleted"]);
                if(taskToUpdate.PercentageCompleted == 100)
                {
                    taskToUpdate.IsCompleted = true;
                    //check if the all tasks of the project are completed?
                    //If yes, call the CalculateTotalCost() method to update the total cost of the project
                }
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Error");
            }
        }

        public ActionResult AddComments(int taskId)
        {
            ProjectTask taskToAddComments = _db.Tasks.First(t => t.Id == taskId);
            if(taskToAddComments != null)
            {
                ViewBag.Task = taskToAddComments;
                
            }
            else
            {
                return NotFound();
            }

            return  View();
        }

        // POST: DevelopmentController/AddCommentForTask/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComments(IFormCollection collection)
        {
            try
            {
                Comment newComment = new Comment();
                newComment.CommentType = (CommentType)int.Parse(collection["CommentType"]);
                newComment.Text = collection["Text"];
                newComment.TaskId = int.Parse(collection["TaskId"]);
                newComment.UserCreator = await _userManager.FindByNameAsync(User.Identity.Name);

                _db.Comments.Add(newComment);
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Error");
            }
        }
    }
}
