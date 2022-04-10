#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Identity;

namespace Finalproject.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public NotificationsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            return View(_db.Notifications.Where(n => n.UserCreator == currentUser).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? ok)
        {
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var allUserNotifications = _db.Notifications.Where(n => n.UserCreator == currentUser).ToList();

            if (allUserNotifications.Any())
            {
                foreach (var notification in allUserNotifications)
                {
                    _db.Notifications.Remove(notification);
                }

                _db.SaveChanges();
            }

            var tasks = _db.Tasks.Where(t => t.UserCreator == currentUser).ToList();

            if (tasks.Any())
            {
                DateTime today = DateTime.Today;

                foreach (var task in tasks)
                {
                    if (task.IsCompleted == false && task.DeadLine == today.AddDays(1))
                    {
                        Notification taNotification = new TaskNotification();
                        taNotification.Title = "Task: " + task.Name + " is one day away from its deadline";
                        taNotification.IsRead = false;
                        taNotification.Description = "This task is only " + task.PercentageCompleted + "% complete";
                        taNotification.TaskId = task.Id;
                        taNotification.UserCreator = currentUser;

                        _db.Notifications.Add(taNotification);
                        _db.SaveChanges();
                    }
                }
            }

            var userProjects = _db.UserProjects.Include(up => up.Project).Where(up => up.UserId == currentUser.Id).ToList();
            List<Project> projects = new List<Project>();

            if (userProjects.Any())
            {
                foreach (var userProject in userProjects)
                {
                    projects.Add(userProject.Project);
                }
            }

            if (projects.Any())
            {
                DateTime today = DateTime.Today;

                foreach (var project in projects)
                {
                    if (project.IsCompleted == false && project.Deadline < today)
                    {
                        Notification pyNotification = new ProjectNotification();
                        pyNotification.Title = "Project: " + project.Title + " has passed its Deadline with unfinished tasks";
                        pyNotification.IsRead = false;
                        pyNotification.Description = "This project has at least one unfinished task";
                        pyNotification.ProjectId = project.Id;
                        pyNotification.UserCreator = currentUser;

                        _db.Notifications.Add(pyNotification);
                        _db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
