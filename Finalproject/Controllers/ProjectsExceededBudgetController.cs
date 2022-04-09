using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Controllers
{
    public class ProjectsExceededBudgetController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ProjectsExceededBudgetController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult ExceedBudgetProjects()
        {
            var project = _db.Projects.Include(u => u.UserProjects).
                          ThenInclude(u => u.User).
                          Where(p => p.TotalCost > p.Budget).ToList();


            return View("ProjectsExceededBudget", project);
        }
    }
}
