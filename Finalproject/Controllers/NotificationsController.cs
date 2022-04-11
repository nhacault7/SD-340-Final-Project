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

            var role = await _userManager.GetRolesAsync(currentUser);
            ViewBag.Role = role[0];

            return View(_db.Notifications.Where(n => n.UserCreator == currentUser).ToList());
        }

        public async Task<IActionResult> UpdateReadStatus(int id)
        {
            var notification = _db.Notifications.First(n => n.Id == id);

            if (notification != null)
            {
                if (notification.IsRead == false)
                {
                    notification.IsRead = true;
                }
                else
                {
                    notification.IsRead = false;
                }
                
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public static async void Update(ApplicationUser currentUser, ApplicationDbContext db)
        {
            var allUserNotifications = db.Notifications.Where(n => n.UserCreator == currentUser).ToList();
            List<Notification> readNotifications = new List<Notification>();

            if (allUserNotifications.Any())
            {
                foreach (var notification in allUserNotifications)
                {
                    if (notification.IsRead == true)
                    {
                        readNotifications.Add(notification);
                    }
                    else
                    {
                        db.Notifications.Remove(notification);
                    }
                }

                db.SaveChanges();
            }

            var userProjects = db.UserProjects.Include(up => up.Project).Where(up => up.UserId == currentUser.Id).ToList();
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

                        bool duplicate = false;
                        foreach (var notification in readNotifications)
                        {
                            if (CheckIfDuplicate(notification, pyNotification, "ProjectNotification") == true)
                            {
                                duplicate = true;
                            }
                        }

                        if (duplicate == false)
                        {
                            db.Notifications.Add(pyNotification);
                            db.SaveChanges();
                        }
                    }
                }
            }

            var tasks = db.Tasks.Where(t => t.UserCreator == currentUser).ToList();

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

                        bool duplicate = false;
                        foreach (var notification in readNotifications)
                        {
                            if (CheckIfDuplicate(notification, taNotification, "TaskNotification") == true)
                            {
                                duplicate = true;
                            }
                        }

                        if (duplicate == false)
                        {
                            db.Notifications.Add(taNotification);
                            db.SaveChanges();
                        }
                    }
                }
            }

            var comments = db.Comments.Where(c => c.UserCreator == currentUser).ToList();

            if (comments.Any())
            {
                foreach (var comment in comments)
                {
                    Notification taNotification = new TaskNotification();
                    taNotification.Title = comment.Text;
                    taNotification.IsRead = false;
                    if (comment.CommentType == CommentType.Urgent)
                    {
                        taNotification.Description = "Urgent comment";
                    }
                    else
                    {
                        taNotification.Description = "Comment";
                    }
                    taNotification.TaskId = comment.TaskId;
                    taNotification.UserCreator= currentUser;

                    bool duplicate = false;
                    foreach (var notification in readNotifications)
                    {
                        if (CheckIfDuplicate(notification, taNotification, "TaskNotification") == true)
                        {
                            duplicate = true;
                        }
                    }

                    if (duplicate == false)
                    {
                        db.Notifications.Add(taNotification);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static bool CheckIfDuplicate(Notification notification, Notification newNotification, string discriminator)
        {
            if (notification.Title == newNotification.Title &&
                notification.Description == newNotification.Description &&
                notification.Discriminator == discriminator)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
