using AspNetCoreGeneratedDocument;
using BenWebApp.Data;
using BenWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Controllers
{
    public class NewItemPharmaController : Controller
    {
        private readonly IDataService<NewItemPharmaModel> _pharmaService;
        
        public NewItemPharmaController(IDataService<NewItemPharmaModel> pharmaService)
        {

            _pharmaService = pharmaService;
        }

        // ✅ Async Index
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;

            var newItem = await _pharmaService.GetAll();
            return View(newItem);
        }

        // ✅ Async Create (GET)
        public async Task<IActionResult> Create()
        {
            var nextCode = await _pharmaService.GetLatestCode();

            var model = new NewItemPharmaModel
            {
                ItemCode = nextCode
            };

            return View(model);
        }

        // ✅ Async Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewItemPharmaModel model)
        {
            if (ModelState.IsValid)
            {
                // Save new Pharma item
                await _pharmaService.AddAsync(model);

                
                // ✅ Send Telegram message
                await _pharmaService.SendTelegramMessageAsync(model);

                return RedirectToAction("Index");

            }

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

            var success = await _pharmaService.CloseItemAsync(id);
            if (!success)
                return NotFound();

            //return RedirectToAction("Index");



            return RedirectToAction(nameof(Index));
        }
    }
}
