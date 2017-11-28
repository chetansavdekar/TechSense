using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechSense.Helpers;
using TechSense.POCO;
using Microsoft.AspNetCore.Authorization;
using TechSense.Filters;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    [Authorize]
    [IsAuthenticatedActionFilter]
    public class DashboardController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index([FromQuery] string CurrentCategoryID, [FromQuery] int? Priority, [FromQuery] int? Top, [FromQuery] string Tag, [FromQuery] string errorCode)
        {
            DashboardModel data = new DashboardModel();

            data.StyleIDForTags = new HashSet<string>();

            data.Categories = CacheHelper.GetCategoryList(Constants.BASE_CATEGORY).Where(cat => cat.Visibility == (int)Visibility.True);
            data.Technologies = new Dictionary<string, IEnumerable<TechnologyEntity>>();
            data.Subcategories = new Dictionary<string, IEnumerable<CategoryEntity>>();

            foreach(CategoryEntity category in data.Categories)
            {
                data.Technologies.Add(category.ID, TableStorageHelper.RetrieveByRangeAsync<TechnologyEntity>(Constants.TABLE_TECHNOLOGY, category.ID, "ge", "T", "lt", "U").Result.Where(tech => tech.Visibility == (int)Visibility.True));

                data.Subcategories.Add(category.ID, CacheHelper.GetCategoryList(category.ID).Where(cat => cat.Visibility == (int)Visibility.True));
            }

            data.TagList = CacheHelper.GetTagList();


            data.CurrentCategoryID = data.Categories.FirstOrDefault(c => c.ID.Equals(CurrentCategoryID?.Trim() ?? ""))?.ID ?? data.Categories.FirstOrDefault()?.ID ?? "";

            data.Priority = Priority ?? 0;

            data.Top = Top?.ToString() ?? "";

            data.Tag = Tag?.Trim() ?? "";


            if (!string.IsNullOrEmpty(errorCode))
            {
                ViewData["errorCode"] = errorCode;
            }

            return View(data);
        }

        private CategoryEntity getCategory(int? categoryID)
        {
            if (categoryID == null)
            {
                return CacheHelper.GetCategoryList(Constants.BASE_CATEGORY).FirstOrDefault();
            }
            else
            {
                return CacheHelper.GetCategoryList(Constants.BASE_CATEGORY).FirstOrDefault(ce => ce.ID == ((int)categoryID).ToString(Constants.PADDING_CATEGORYID));
            }
        }
    }
}
