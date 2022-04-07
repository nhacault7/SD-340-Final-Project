using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Finalproject.Controllers
{
    public class TaskHelperController : Controller
    {
        // GET: TaskHelperController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TaskHelperController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TaskHelperController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskHelperController/Create
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

        // GET: TaskHelperController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskHelperController/Edit/5
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

        // GET: TaskHelperController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskHelperController/Delete/5
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
