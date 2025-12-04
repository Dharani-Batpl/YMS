using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    public class AccountController : Controller
    {

        private readonly v1Client _apiClient;
        public AccountController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        // ============================
        // CHANGE PASSWORD - GET
        // ============================
        [HttpGet]
        public IActionResult ChangePassword(string username = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                username = HttpContext.Session.GetString("LoginUser");
            }
            TempData["Success"] = null;
            return View(new YardManagementApplication.Models.ChangePasswordRequest
            {
                Username = username
            });

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(YardManagementApplication.Models.ChangePasswordRequest model)
        {
            string controller = nameof(AccountController);
            string action = nameof(ChangePassword);

            try
            {
                // Basic MVC validation
                if (!ModelState.IsValid)
                    return View(model);

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                //if (model.OldPassword == model.NewPassword)
                //{
                //    ModelState.AddModelError("", "New password cannot be same as old password.");
                //    return View(model);
                //}

                // Build API request
                var apiRequest = new ChangePasswordRequest
                {
                    Username = model.Username,
                 
                    NewPassword = model.NewPassword
                };

                // Call backend API
                var result = await _apiClient.ChangePasswordAsync(apiRequest);

                // Backend says error
                if (result.Status != 200)
                {
                    ModelState.AddModelError("", result.Detail ?? "Failed to update the password.");
                    return View(model);
                }

                // SUCCESS
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                

                // API exception handling
                if (ex is ApiException<ResponseModel> apiEx && apiEx.Result != null)
                {
                    ModelState.AddModelError("", apiEx.Result.Detail);
                    return View(model);
                }

                // Fallback
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

    }
}