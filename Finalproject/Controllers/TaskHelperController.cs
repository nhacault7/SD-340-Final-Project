using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Controllers
{
    public class TaskHelperController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public TaskHelperController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: TaskHelperController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Assign(int projectId, int taskId)
        {
            ProjectTask taskToAssign = _db.Tasks.Include(t=>t.UserCreator).First(t => t.Id == taskId);
            Project currProject = _db.Projects.First(p => p.Id == projectId);
            if ( taskToAssign != null && currProject != null )
            {
                //manager cannot assign task that already has a developer
                if ( taskToAssign.UserCreator!= null )
                {
                    return RedirectToAction("Details", "ProjectHelper", new
                    {
                        projectId = currProject.Id
                    });

                }
                else if ( taskToAssign.ProjectId == currProject.Id )
                {
                    var developerRole = await _roleManager.FindByNameAsync("Developer");
                    //find all users with developer role
                    var developerIds = _db.UserRoles.Where(u => u.RoleId == developerRole.Id).ToList();
                    //create and pass a list of developers to view
                    List<ApplicationUser> developers = new List<ApplicationUser>();
                    var developerList = new List<SelectListItem>();
                    foreach ( var item in developerIds )
                    {
                        ApplicationUser developer = await _userManager.FindByIdAsync(item.UserId);
                        developerList.Add(new SelectListItem
                        {
                            Value = developer.Id,
                            Text = developer.UserName
                        });
                    }
                    ViewBag.Developers = developerList;
                //    SelectList selectlist = new SelectList(developers, "Id", "UserName");     
                //   ViewBag.Developers = selectlist;
                    ViewBag.Task = taskToAssign;
                    return View();
                }
                else
                {
                    return BadRequest();
                }
         
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int taskId, string developerId)
        {
           ApplicationUser developer = await _userManager.FindByIdAsync(developerId);
           ProjectTask taskToAssign =  _db.Tasks.First(t => t.Id == taskId);
            if ( developer != null && taskToAssign != null )
            {
                taskToAssign.UserCreator = developer;
                _db.SaveChanges();
                return RedirectToAction("Details", "ProjectHelper", new
                {
                    projectId = taskToAssign.ProjectId
                });
            }
            else
            {
                return NotFound();
            }
           

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
        public async Task<ActionResult> Edit(int taskId)
        {
            ProjectTask currTask = _db.Tasks.First(t => t.Id == taskId);
            if ( currTask != null )
            {
                string currUserName = User.Identity.Name;
                ApplicationUser projectManager = await _userManager.FindByNameAsync(currUserName);
                //to check if the current project belongs to the corresponding project manager
                var belongsToTheManager = _db.UserProjects.Where(u => u.UserId == projectManager.Id).Any(u => u.ProjectId == currTask.ProjectId);

                if ( belongsToTheManager )
                {
                    ViewBag.projectId = currTask.ProjectId;
                    List<SelectListItem> priorities = new List<SelectListItem>
                    {
                        new SelectListItem(){ Text = "Urgent", Value = Priority.Urgent.ToString() },
                        new SelectListItem(){ Text = "High", Value = Priority.High.ToString() },
                        new SelectListItem(){ Text = "Medium", Value = Priority.Medium.ToString() },
                        new SelectListItem(){ Text = "Low", Value = Priority.Low.ToString() }
                    };
                    ViewBag.priorityList = priorities;
                    return View(currTask);
                }
                else
                {
                    return RedirectToAction("Details", "ProjectHelper", new
                    {
                        projectId = currTask.ProjectId
                    });
                }
            }
            else
            {
                return NotFound();
            }
        }

        // POST: TaskHelperController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, IFormCollection collection)
        {
            try
            {
                ProjectTask taskToBeEdited = _db.Tasks.First(t => t.Id == Id);
            
                taskToBeEdited.Name = collection["Name"] !=""? collection["Name"]:"" ;
                taskToBeEdited.PercentageCompleted = collection["PercentageCompleted"]!=""? double.Parse(collection["PercentageCompleted"].ToString()):0;
                taskToBeEdited.IsCompleted = collection["IsCompleted"]!=""? bool.Parse(collection["IsCompleted"]):false;
                taskToBeEdited.Priority = (Priority)Enum.Parse(typeof(Priority), collection["Priority"].ToString());
                taskToBeEdited.EndDate = collection["EndDate"]!=""? DateTime.Parse(collection["EndDate"]):null;
                taskToBeEdited.DeadLine = DateTime.Parse(collection["DeadLine"]);

                _db.SaveChanges();

                return RedirectToAction("Details", new
                {
                    taskId = taskToBeEdited.Id
                });
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskHelperController/Delete/5
        public async Task<ActionResult> Delete(int taskId )
        {
            ProjectTask currTask = _db.Tasks.First(t => t.Id == taskId);
            if ( currTask != null )
            {
                string currUserName = User.Identity.Name;
                ApplicationUser projectManager = await _userManager.FindByNameAsync(currUserName);
                //to check if the current project belongs to the corresponding project manager
                var belongsToTheManager = _db.UserProjects.Where(u => u.UserId == projectManager.Id).Any(u => u.ProjectId == currTask.ProjectId);

                if ( belongsToTheManager )
                {
                    ViewBag.ProjectId = currTask.ProjectId;
                    return View(currTask);
                }
                else
                {
                    return RedirectToAction("Details", "ProjectHelper", new
                    {
                        projectId = currTask.ProjectId
                    });
                }
            }
            else
            {
                return NotFound();
            }
        }

        // POST: TaskHelperController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id, IFormCollection collection)
        {
            try
            {
                ProjectTask taskToDelete = _db.Tasks.First(t => t.Id == Id);
                if ( taskToDelete != null )
                {
                    var currTaskProjectId = taskToDelete.ProjectId;
                    _db.Remove(taskToDelete);
                    _db.SaveChanges();
                    return RedirectToAction("Details", "ProjectHelper", new
                    {
                        projectId = currTaskProjectId
                    });
                    ;

                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
