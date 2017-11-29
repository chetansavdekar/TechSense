using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechSense.Filters;
using TechSense.POCO;
using TechSense.Helpers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    [Authorize(Policy = "FullAccess")]
    [IsAuthenticatedActionFilter]
    public class UserController : Controller
    {
        public IActionResult Index([FromQuery] string errorCode)
        {
            UserViewModel data = new POCO.UserViewModel();

            data.Users = CacheHelper.GetUsers();
            data.CurrentUsernameHash = (HttpContext?.User?.Identity?.Name ?? "").Trim().ToLower();

            if (!string.IsNullOrEmpty(errorCode))
            {
                ViewData["errorCode"] = errorCode;
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult Add(UserEntity user)
        {
            int errorCode = 0;
            try
            {
                TableStorageHelper.InsertAsync(Constants.TABLE_USER, user).Wait();

                CacheHelper.ClearUsersCache();
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { errorCode = errorCode });
        }

        [HttpPost]
        public IActionResult Edit(UserEntity user)
        {
            int errorCode = 0;

            try
            {
                if ((HttpContext?.User?.Identity?.Name ?? "").Trim().ToLower().Equals((user?.RowKey?.Trim()?.ToLower() ?? "Q!@QQW@!")) ||
                    (HttpContext?.User?.Identity?.Name ?? "") == "sachin")
                {
                    user.ETag = "*";
                    TableStorageHelper.MergeAsync(Constants.TABLE_USER, user).Wait();

                    CacheHelper.ClearUsersCache();
                }
                else
                {
                    errorCode = Constants.ERROR_CODE_ACCESS_DENIED;
                }
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { errorCode = errorCode });
        }
    }
}
