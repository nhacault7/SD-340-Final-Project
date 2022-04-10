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
    public class CommentsController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public CommentsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Comments/Create
        public IActionResult Create(int taskId)
        {
            ViewBag.TaskId = taskId;

            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection, int taskId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var task = _db.Tasks.Include(t => t.Project).First(t => t.Id == taskId);
                    var userProjects = _db.UserProjects.Include(up => up.Project).First(up => up.ProjectId == task.Project.Id);
                    ApplicationUser projectManager = await _userManager.FindByIdAsync(userProjects.UserId);

                    Comment comment = new Comment();
                    comment.Text = collection["Text"].ToString();
                    comment.CommentType = CommentType.Urgent;
                    comment.TaskId = taskId;
                    comment.Task = task;
                    comment.UserCreator = projectManager;
                    _db.Add(comment);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Development");
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
