using BenWebApp.Data;
using BenWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Controllers
{
    public class NewItemCSRController : Controller
    {
        private readonly IDataService<NewItemCSRModel> _csrService;
        public NewItemCSRController(IDataService<NewItemCSRModel> csrService)
        {
            _csrService = csrService;
        }
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;

            var newItem = await _csrService.GetAll();
            return View(newItem);
        }
        public async Task<IActionResult> Create()
        {
            var nextCode = await _csrService.GetLatestCode();

            var model = new NewItemCSRModel
            {
                ItemCode = nextCode
            };

            return View(model);
        }
        // ✅ Async CloseItem
        public async Task<IActionResult> CloseItem(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "IT")
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _csrService.CloseItemAsync(id);
            if (!success)
                return NotFound();

            //return RedirectToAction("Index");



            return RedirectToAction(nameof(Index));

            
        }
    }
}
