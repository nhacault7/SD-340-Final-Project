using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Controllers
{
    public class ProjectHelperController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public ProjectHelperController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // GET: ProjectHelperController

        public ActionResult Index()
        {
            var currUserName = User.Identity.Name;
            
            var UserProjectList =  _db.UserProjects.Where(up => up.User.UserName == currUserName).ToList();

            var projectList = _db.Projects.Include(p=>p.UserProjects).ToList();

            return View(projectList);
        }

        // GET: ProjectHelperController/Details/5
        public ActionResult Details(int projectId)
        {
            ViewBag.tasks = _db.Tasks.Include(t=>t.UserCreator).Where(t => t.ProjectId == projectId).ToList();
            var currProject =  _db.Projects.Where(p=>p.Id == projectId).First();       
            return View(currProject);
        }

        // GET: ProjectHelperController/Create
        public ActionResult Create()
        {
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

        // POST: ProjectHelperController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    var currUsername = User.Identity.Name;
                    ApplicationUser currUser = await _userManager.FindByEmailAsync(currUsername);
                    //add new project to db
                    Project project = new Project();
                    project.Title = collection["Title"].ToString();
                    project.Description = collection["Description"].ToString();
                    project.StartDate = DateTime.Today;
                    project.PercentageCompleted = 0;
                    project.TotalCost = 0;
                    project.IsCompleted = false;
                    project.Deadline = DateTime.Parse(collection["Deadline"]);
                    project.Priority = (Priority)Enum.Parse(typeof(Priority),collection["Priority"].ToString());
                    project.Budget = double.Parse(collection["Budget"]);
                    project.IsCompleted = false; 
                    _db.Projects.Add(project);
                   

                    //add userProject to track user-project relationship
                    UserProject userProject = new UserProject();
                    userProject.ProjectId = project.Id;
                    userProject.Project = project;
                    userProject.UserId = currUser.Id;
                    userProject.User = currUser;
                    _db.UserProjects.Add(userProject);

                    _db.SaveChanges();

                    return RedirectToAction("Index","Dashboard");
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

        // GET: ProjectHelperController/Edit/5
        public async Task<ActionResult> Edit(int projectId)
        {
           UserProject currProject = _db.UserProjects.Where(up => up.ProjectId == projectId).First();
           ApplicationUser currUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            if ( currProject.UserId == currUser.Id )
            {
                List<SelectListItem> priorities = new List<SelectListItem>
            {
                new SelectListItem(){ Text = "Urgent", Value = Priority.Urgent.ToString() },
                new SelectListItem(){ Text = "High", Value = Priority.High.ToString() },
                new SelectListItem(){ Text = "Medium", Value = Priority.Medium.ToString() },
                new SelectListItem(){ Text = "Low", Value = Priority.Low.ToString() }
            };
                ViewBag.priorityList = priorities;
                Project project =  _db.Projects.Where(p => p.Id == currProject.ProjectId).First();
                return View(project);
            }
            else
            {
                TempData["msg"] = "Sorry, only the project owner can edit this project.";
                return RedirectToAction("Index","Dashboard");
            }

        }

        // POST: ProjectHelperController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, IFormCollection collection)
        {
            try
            {
                ModelState.Remove("TotalCost");
                ModelState.Remove("PercentageCompleted");
                ModelState.Remove("IsCompleted");
                if ( ModelState.IsValid )
                {
                    Project projectToUpdate = _db.Projects.Where(p => p.Id == Id).First();
                    projectToUpdate.Title = collection["Title"].ToString();
                    projectToUpdate.Description = collection["Description"].ToString();
                    projectToUpdate.PercentageCompleted = double.Parse(collection["PercentageCompleted"].ToString());
                    projectToUpdate.IsCompleted = bool.Parse(collection["IsCompleted"].ToString());
                    projectToUpdate.Budget = double.Parse(collection["Budget"].ToString());
                    projectToUpdate.TotalCost = float.Parse(collection["TotalCost"].ToString());
                    projectToUpdate.Priority = (Priority)Enum.Parse(typeof(Priority), collection["Priority"].ToString());
                    projectToUpdate.StartDate = DateTime.Parse(collection["StartDate"]);
                    projectToUpdate.EndDate = DateTime.Parse(collection["EndDate"]);
                    projectToUpdate.Deadline = DateTime.Parse(collection["Deadline"]);

                    _db.SaveChanges();
                    return RedirectToAction("Index", "Dashboard");
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

        // GET: ProjectHelperController/Delete/5
        public ActionResult Delete(int projectId)
        {
            Project projectToDelete =   _db.Projects.First(p => p.Id == projectId);
            return View(projectToDelete);
        }

        // POST: ProjectHelperController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id, IFormCollection collection)
        {
            try
            {
              var projectToDelete =  _db.Projects.First(p => p.Id == Id);
              var userProjectDelete=   _db.UserProjects.First(up => up.ProjectId == Id);
              var tasksDelete =  _db.Tasks.Where(t => t.ProjectId == Id).ToList();
            
                //remove taskes of project
                _db.Tasks.RemoveRange(tasksDelete);
                //remove project-user relationship
                _db.UserProjects.Remove(userProjectDelete);
                //remove project
                _db.Projects.Remove(projectToDelete);
                _db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View();
            }
        }

    }
}
