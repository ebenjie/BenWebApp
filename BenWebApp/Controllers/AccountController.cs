using Microsoft.AspNetCore.Mvc;

namespace BenWebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public async Task<IActionResult> Login()
        {
            // Nothing async here, but keeps consistency
            return await Task.FromResult(View());
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 🔒 For demo only. Replace with real authentication (Identity/DB).
            if (username == "ace" && password == "12345")
            {
                // Store in session for now
                HttpContext.Session.SetString("UserRole", "IT");

                return await Task.FromResult(
                    RedirectToAction("Index", "NewItemPharma")
                );
            }

            ViewBag.Error = "Invalid username or password";
            return await Task.FromResult(View());
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            return await Task.FromResult(
                RedirectToAction("Login")
            );
        }
    }
}
