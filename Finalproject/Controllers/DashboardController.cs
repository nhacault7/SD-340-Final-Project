using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class DashBoardController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public DashBoardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: DashBoardController
        public async Task<ActionResult> Index(string searchString, int? pageNumber)
        {     
            //Get all projects only belonged to the current logged in PM
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            List<UserProject> userProjects = _db.UserProjects.Include(up => up.Project).Where(up => up.UserId == currentUser.Id).ToList();
            List<int> projectIdArr = new List<int>();
            foreach (var userProject in userProjects)
            {
                projectIdArr.Add(userProject.ProjectId);
            }
            var allProjectsOfPM = _db.Projects.Include(p => p.Notifications).Where(p => projectIdArr.Contains(p.Id)).OrderBy(p => p.Priority);

            int count = 0;
            foreach (var project in allProjectsOfPM)
            {
                count += project.Notifications.Count();
            }
            ViewData["Count"] = count;

            //Pagination
            int pageSize = 10;//the max value is 10 records in every page
            //get filter items when search string is not null
            if (searchString != null)
            {
                var searchedProject = allProjectsOfPM.Where(p => p.Title.Contains(searchString));

                //converts the project query to a single page of projects in a collection type that supports paging.
                //The AsNoTracking() extension method returns a new query and the returned entities will not be cached by the context (DbContext or Object Context). 
                return View(await PaginatedList<Project>.CreateAsync(searchedProject.AsNoTracking(), pageNumber ?? 1, pageSize)); //if pageNumber is null, pageNumber=1,else pageNumber = pageNumber
            }

            return View(await PaginatedList<Project>.CreateAsync(allProjectsOfPM.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: DashBoardController/Details/5
        public ActionResult ProjectDetails(int projectId)
        {
            Project project = _db.Projects.Include(p => p.Tasks).ThenInclude(t => t.UserCreator).First(p => p.Id == projectId);
            return View(project);
        }

        // GET: DashBoardController/OverdueTasks
        public async Task<ActionResult> OverdueTasks()
        {
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            List<UserProject> userProjects = _db.UserProjects.Include(up => up.Project).Where(up => up.UserId == currentUser.Id).ToList();
            List<int> projectIdArr = new List<int>();
            foreach (var userProject in userProjects)
            {
                projectIdArr.Add(userProject.ProjectId);
            }
            List<ProjectTask> overdueTasks = _db.Tasks.Where(t => projectIdArr.Contains(t.ProjectId) && t.IsCompleted == false && DateTime.Today > t.DeadLine).ToList();
            return View(overdueTasks);
        }

        // POST: DashBoardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: DashBoardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashBoardController/Edit/5
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

        // GET: DashBoardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashBoardController/Delete/5
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
