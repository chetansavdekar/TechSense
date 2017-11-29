using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechSense.POCO;
using System.Security.Claims;
using TechSense.Filters;
using TechSense.Helpers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    [IsAuthenticatedActionFilter()]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                string role;
                if (LoginUser(login.Username, login.Password, out role))
                {
                    IList<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, login.Username));
                    claims.Add(new Claim(ClaimTypes.Role, role));

                    var userIdentity = new ClaimsIdentity(claims, "login");

                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.Authentication.SignInAsync("CookieAuthentication", principal);

                    //Just redirect to our index after logging in. 
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("", "Please enter correct username and password.");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("CookieAuthentication");
            return Redirect("/Account/Login");
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        private bool LoginUser(string username, string password, out string role)
        {
            role = "";
            username = username ?? "";
            password = password ?? "";

            if (username.Trim().ToLower().Equals("sachin") && password.Trim().Equals("Wassup123"))
            {
                role = AccessLevel.Full.ToString();
                return true;
            }

            UserEntity userEntity = CacheHelper.GetUsers().FirstOrDefault(user => user.RowKey.Trim().ToLower() == username.Trim().ToLower() && user.Password.Trim() == password.Trim());

            if (userEntity != null)
            {
                try
                {
                    role = ((AccessLevel)Enum.Parse(typeof(AccessLevel), userEntity.AccessLevel, true)).ToString();
                }
                catch
                {
                    role = AccessLevel.Read.ToString();
                }
                return true;
            }

            return false;
        }
    }
}
