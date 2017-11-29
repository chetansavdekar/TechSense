using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechSense.Helpers;
using TechSense.POCO;
using TechSense.Filters;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    [Authorize(Policy = "FullAccess")]
    [IsAuthenticatedActionFilter]
    public class CategoryController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string category, [FromQuery] string errorCode)
        {
            category = category?.Trim() ?? Constants.BASE_CATEGORY;

            IEnumerable<CategoryEntity> categories = null;

            categories = CacheHelper.GetCategoryList(category);
            ViewData["category"] = category;
            if (category.Equals(Constants.BASE_CATEGORY))
            {
                ViewData["categoryName"] = Constants.BASE_CATEGORY;
            }
            else
            {
                ViewData["categoryName"] = CacheHelper.GetCategoryList().FirstOrDefault(cat => cat.ID.Equals(category))?.Name ?? "";
            }
            

            if (!string.IsNullOrEmpty(errorCode))
            {
                ViewData["errorCode"] = errorCode;
            }

            return View(categories);
        }

        public IActionResult Add(CategoryEntity entity)
        {
            int errorCode = 0;

            try
            {
                if (CacheHelper.GetCategoryList(entity.PartitionKey).Count(cat => cat.NameHash.Equals(entity.NameHash)) > 0)
                {
                    errorCode = Constants.ERROR_CODE_ENTITY_ALREADY_EXISTS;
                }
                else
                {
                    string nextSortNumber = SequenceHelper.GetNextSequence(Constants.SORT, Constants.TABLE_CATEGORY + "_" + entity.PartitionKey, Constants.PADDING_SORT);

                    string nextID = SequenceHelper.GetNextSequence(Constants.ID, Constants.TABLE_CATEGORY, Constants.PADDING_CATEGORYID);

                    entity.RowKey = nextSortNumber + "_" + nextID;


                    TableStorageHelper.InsertOrMergeAsync(Constants.TABLE_CATEGORY, entity).Wait();

                    CacheHelper.ClearCategoryListCache();
                }
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }


            return RedirectToAction("Index", new { category = entity.PartitionKey, errorCode = errorCode });
        }

        public IActionResult Edit(CategoryEntity entity)
        {
            int errorCode = 0;

            try
            {
                if (CacheHelper.GetCategoryList(entity.PartitionKey).Count(cat => cat.NameHash.Equals(entity.NameHash) && !cat.ID.Equals(entity.ID)) > 0)
                {
                    errorCode = Constants.ERROR_CODE_ENTITY_ALREADY_EXISTS;
                }
                else
                {
                    TableStorageHelper.MergeAsync(Constants.TABLE_CATEGORY, entity).Wait();

                    CacheHelper.ClearCategoryListCache();
                }
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { category = entity.PartitionKey, errorCode = errorCode });
        }

        //public IActionResult Delete(CategoryEntity entity)
        //{
        //    int errorCode = 0;

        //    try
        //    {
        //        entity.Visibility = false;
        //        TableStorageHelper.MergeAsync(Constants.TABLE_CATEGORY, entity).Wait();

        //        CacheHelper.ClearCategoryListCache();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!TableStorageHelper.IsStorageException(ex, out errorCode))
        //        {
        //            errorCode = Constants.ERROR_CODE_COMMON;
        //        }
        //    }

        //    return RedirectToAction("Index", new { category = entity.PartitionKey, errorCode = errorCode });
        //}

        public IActionResult Sort(CategoryEntity up, CategoryEntity down)
        {
            int errorCode = 0;

            try
            {
                CategoryEntity newUp = new CategoryEntity(up.PartitionKey, up.RowKey);
                CategoryEntity newDown = new CategoryEntity(down.PartitionKey, down.RowKey);

                up.CopyTo(newUp, false);
                down.CopyTo(newDown, false);

                string upSortNumber = up.RowKey.Split('_')[0];
                string downSortNumber = down.RowKey.Split('_')[0];

                newUp.RowKey = downSortNumber + "_" + newUp.ID;
                newDown.RowKey = upSortNumber + "_" + newDown.ID;

                TableStorageHelper.BatchAsync(Constants.TABLE_CATEGORY, new CategoryEntity[] { up, down }, new CategoryEntity[] { newUp, newDown }, null).Wait();

                CacheHelper.ClearCategoryListCache();
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { category = up.PartitionKey, errorCode = errorCode });
        }
    }
}
