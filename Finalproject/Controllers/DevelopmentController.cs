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

        // GET: DevelopmentController/Details/5
        public ActionResult Details(int taskId)
        {
            return View();
        }


        // GET: DevelopmentController/Edit/5
        public ActionResult TaskEdit(int taskId)
        {
            return View();
        }

        // POST: DevelopmentController/Edit/5
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

    }
}
