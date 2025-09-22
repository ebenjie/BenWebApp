using BenWebApp.Data;
using BenWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Controllers
{
    public class NewItemCSRController : Controller
    {
        private readonly DataContext _context;
        public NewItemCSRController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            var latestCode = await _context.CodePerDepts
                                           .Where(c => c.Department == "CSR")
                                           .OrderByDescending(c => c.Id)
                                           .Select(c => c.Code)
                                           .FirstOrDefaultAsync();

            // If no code exists yet, start from a base value (example: 4000000000000)
            var nextCode = latestCode == 0 ? 4000000000000 : latestCode + 1;

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

            var item = await _context.NewItemCSR.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.IT_Status = "Closed";
            _context.NewItemCSR.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
