using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using YardManagementApplication.Models;

namespace YardManagementApplication
{
    public class HomeController : Controller
    {
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

     
    }
}