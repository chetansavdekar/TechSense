using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechSense.POCO;
using TechSense.Helpers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    public class TagController : Controller
    {
        // GET: /<controller>/
        public ActionResult Get()
        {
            return Json(CacheHelper.GetTagList().Select(tag => new { label = tag.RowKey, value = tag.ID }));
        }
    }
}
