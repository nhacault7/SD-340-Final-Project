using Finalproject.Data;
using Finalproject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Finalproject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserManagerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: UserManagerController
        public ActionResult Index()
        {
            List<ApplicationUser> users = _db.Users.ToList();
            return View(users);
        }

        // GET: UserManagerController/UserRoleDetails/5
        public async Task<ActionResult> UserRoleDetails(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            return View(roles);
        }

        // GET: UserManagerController/AssignRoleToUser
        public async Task<ActionResult> AssignRoleToUser(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = user.Id;
            var rolesOfUser = await _userManager.GetRolesAsync(user);
            List<IdentityRole> allRoles = _db.Roles.ToList();
            //get the rols not already assigned to the user
            List<IdentityRole> rolesToAssign = allRoles.Where(a => !rolesOfUser.Contains(a.Name)).ToList();
            SelectList rolesSlectList = new SelectList(rolesToAssign, "Id", "Name");
            return View(rolesSlectList);
        }

        // POST: UserManagerController/AssignRoleToUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRoleToUser(string userId, string roleId)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                IdentityRole role = await _roleManager.FindByIdAsync(roleId);

                await _userManager.AddToRoleAsync(user, role.Name);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserManagerController/Delete/5
        public ActionResult DeleteRole(string userId)
        {
            return View();
        }

        // POST: UserManagerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRole(string userId, string roleId)
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
