using BenWebApp.Data;
using BenWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Controllers
{
    public class NewItemPharmaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly NewCodePharmaContext _context;
        public NewItemPharmaController(NewCodePharmaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Async Index
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;

            var newItem = await _context.NewItemPharma
                                        .Where(x => x.IT_Status == "Open")
                                        .ToListAsync();

            return View(newItem);
        }

        // ✅ Async Create (GET)
        public async Task<IActionResult> Create()
        {
            var latestCode = await _context.CodePerDepts
                                           .Where(c => c.Department == "Pharmacy")
                                           .OrderByDescending(c => c.Id)
                                           .Select(c => c.Code)
                                           .FirstOrDefaultAsync();

            // If no code exists yet, start from a base value (example: 4000000000000)
            var nextCode = latestCode == 0 ? 4000000000000 : latestCode + 1;

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
                await _context.NewItemPharma.AddAsync(model);

                // Look for existing Pharmacy record
                var existingCode = await _context.CodePerDepts
                                                 .FirstOrDefaultAsync(c => c.Department == "Pharmacy");

                if (existingCode != null)
                {
                    // Update existing record
                    existingCode.Code = model.ItemCode;
                    _context.CodePerDepts.Update(existingCode);
                }
                else
                {
                    // If no record exists yet, create one
                    var codeTrack = new CodePerDeptModel
                    {
                        Department = "Pharmacy",
                        Code = model.ItemCode
                    };
                    await _context.CodePerDepts.AddAsync(codeTrack);
                }

                await _context.SaveChangesAsync();

                //send to telegram
                await SendTelegramMessage(model);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private async Task SendTelegramMessage(NewItemPharmaModel item)
        {
            var botToken = _configuration["Telegram:BotToken"];
            var chatId = _configuration["Telegram:ChatId"];

            using var client = new HttpClient();
            var message = $"New Pharma Item Added\n\n" +
                          $"ItemCode: {item.ItemCode}\n" +
                          $"Description: {item.Description}\n" +
                          $"Generic: {item.GenericName}\n" +
                          $"SmallUnit: {item.SmallUnit}\n" +
                          $"BigUnit: {item.BigUnit}\n" +
                          $"Price: {item.SellingPrice}\n" +
                          $"Requested By: {item.RequestedBy}\n" +
                          $"Date: {item.RequestDate:MM-dd-yyyy}";

            var url = $"https://api.telegram.org/bot{botToken}/sendMessage" +
                      $"?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            await client.GetAsync(url);
        }



        // ✅ Async CloseItem
        public async Task<IActionResult> CloseItem(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "IT")
            {
                return RedirectToAction("Login", "Account");
            }

            var item = await _context.NewItemPharma.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.IT_Status = "Closed";
            _context.NewItemPharma.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
