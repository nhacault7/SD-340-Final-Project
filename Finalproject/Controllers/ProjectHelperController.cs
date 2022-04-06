using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
         
            //ICollection<Project> projectList = new List<Project>();
            //foreach ( var userProject in UserProjectList )
            //{
            //    projectList.Add( userProject.Project );
            //}


            return View(projectList);
        }

        // GET: ProjectHelperController/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.tasks = _db.Tasks.Include(t=>t.UserCreator).Where(t => t.ProjectId == id).ToList();
            //InvalidOperationException: Unable to materialize entity instance of type 'Notification'.
            //No discriminators matched the discriminator value 'Project'.
            
           // ViewBag.notifications = _db.Notifications.Where(n=>n.ProjectId == id ).ToList();
            var currProject =  _db.Projects.Where(p=>p.Id == id).First();       
            return View(currProject);
        }

        // GET: ProjectHelperController/Create
        public ActionResult Create()
        {
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
                    project.StartDate = DateTime.Parse(collection["StartDate"]);
                    project.Deadline = DateTime.Parse(collection["Deadline"]);
                    project.Budget = double.Parse(collection["Budget"]);
                    _db.Projects.Add(project);
                   

                    //add userProject to track user-project relationship
                    UserProject userProject = new UserProject();
                    userProject.ProjectId = project.Id;
                    userProject.Project = project;
                    userProject.UserId = currUser.Id;
                    userProject.User = currUser;
                    _db.UserProjects.Add(userProject);

                    _db.SaveChanges();

                    return RedirectToAction(nameof(Index));
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
        public async Task<ActionResult> Edit(int id)
        {
           UserProject currProject = _db.UserProjects.Where(up => up.ProjectId == id).First();
           ApplicationUser currUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            if ( currProject.UserId == currUser.Id )
            {
              Project project =  _db.Projects.Where(p => p.Id == currProject.ProjectId).First();
                return View(project);
            }
            else
            {
                TempData["msg"] = "Sorry, only the project owner can edit this project.";
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: ProjectHelperController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    Project projectToUpdate = _db.Projects.Where(p => p.Id == id).First();
                    projectToUpdate.Title = collection["Title"].ToString();
                    projectToUpdate.Description = collection["Description"].ToString();
                    projectToUpdate.PercentageCompleted = double.Parse(collection["PercentageCompleted"]);
                    projectToUpdate.IsCompleted = bool.Parse(collection["IsCompleted"].ToString());
                    projectToUpdate.Budget = double.Parse(collection["Budget"].ToString());
                    projectToUpdate.TotalCost = collection["TotalCost"].ToString();
                    projectToUpdate.Priority = int.Parse(collection["Priority"]);
                    projectToUpdate.StartDate = DateTime.Parse(collection["StartDate"]);
                    projectToUpdate.EndDate = DateTime.Parse(collection["EndDate"]);
                    projectToUpdate.Deadline = DateTime.Parse(collection["Deadline"]);

                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectHelperController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
              var projectToDelete =  _db.Projects.First(p => p.Id == id);
              var userProjectDelete=   _db.UserProjects.First(up => up.ProjectId == id);
              var tasksDelete =  _db.Tasks.Where(t => t.ProjectId == id).ToList();
            

                _db.Tasks.RemoveRange(tasksDelete);
                _db.UserProjects.Remove(userProjectDelete);
                _db.Projects.Remove(projectToDelete);
                _db.SaveChanges();
              return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult createTask(int projectId)
        {
            return RedirectToAction("Create","TaskHelper",projectId);
        }
    }
}
