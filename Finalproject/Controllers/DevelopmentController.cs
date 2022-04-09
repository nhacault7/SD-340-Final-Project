﻿using Finalproject.Data;
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
        public async Task<ActionResult> UpdateTask(int id, IFormCollection collection)
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
                    taskToUpdate.EndDate = DateTime.Now;

                    Project project = _db.Projects.Include(p => p.Tasks).Include(p => p.UserProjects).ThenInclude( up => up.User).First(p => p.Id == taskToUpdate.ProjectId);

                    //check if the all tasks of the project are completed?
                    //If yes, calculate the TotalCost to update the value total cost of the project
                    if(project.Tasks.All(t => t.IsCompleted == true))
                    {
                        float totalCost = 0;
                        float projectCost = 0;
                        float tasksTotalCost = 0;
                        string pmUserId = "";
                    
                        //Get PM's UserId
                        foreach (var teamMember in project.UserProjects)
                        {
                            if (await _userManager.IsInRoleAsync(teamMember.User, "Project Manager"))
                            {
                                pmUserId = teamMember.UserId;
                            }
                        }
                        //Get the PM's project salary
                        ApplicationUser user = await _userManager.FindByIdAsync(pmUserId);
                        projectCost = user.DailySalary;

                        //Get the cost of all tasks
                        var tasks = _db.Tasks.Where(p => p.ProjectId == taskToUpdate.ProjectId).ToList();
                        foreach (var task in tasks)
                        {
                            user = await _userManager.FindByIdAsync(task.UserCreator.Id);
                            DateTime endDate = DateTime.Now;

                            if ((bool)task.IsCompleted && task.EndDate != null)
                            {
                                endDate = (DateTime)task.EndDate;   
                            }

                            TimeSpan totalDays = (TimeSpan)(endDate - task.StartDate);
                            tasksTotalCost += (totalDays.Days + 1) * user.DailySalary;
                        }

                        totalCost = tasksTotalCost + projectCost;

                        //set the TotalCost of the project
                        project.TotalCost = totalCost;
                        project.IsCompleted = true;
                        project.PercentageCompleted = 100;
                        project.EndDate = DateTime.Now;
                    }

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
