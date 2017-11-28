using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechSense.POCO;
using TechSense.Helpers;
using Newtonsoft.Json;
using TechSense.Filters;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TechSense.Controllers
{
    [Authorize]
    [IsAuthenticatedActionFilter]
    public class TechnologyController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index([FromQuery] int? category, [FromQuery] int? subcategory, [FromQuery] int? technology, [FromQuery] string errorCode)
        {
            TechnologyViewModel data = new POCO.TechnologyViewModel();
            data.CategoryID = category?.ToString(Constants.PADDING_CATEGORYID);
            data.SubcategoryID = subcategory?.ToString(Constants.PADDING_CATEGORYID);
            data.TechnologyID = technology?.ToString(Constants.PADDING_TECHNOLOGYID);

            if (category != null)
            {
                string value1 = (subcategory != null ? "T_" + ((int)subcategory).ToString(Constants.PADDING_CATEGORYID) : "T");

                string value2 = (subcategory != null ? "T_" + ((int)subcategory + 1).ToString(Constants.PADDING_CATEGORYID) : "U");

                data.Technologies = TableStorageHelper.RetrieveByRangeAsync<TechnologyEntity>(Constants.TABLE_TECHNOLOGY, ((int)category).ToString(Constants.PADDING_CATEGORYID), "ge", value1, "lt", value2).Result;

                data.Subcategories = CacheHelper.GetCategoryList(((int)category).ToString(Constants.PADDING_CATEGORYID));

                data.TagList = CacheHelper.GetTagList();

                data.Active = new Microsoft.AspNetCore.Mvc.Rendering.SelectListGroup() { Name = "Active" };
                data.Inactive = new Microsoft.AspNetCore.Mvc.Rendering.SelectListGroup() { Name = "Inactive" };

                data.CategoryName = CacheHelper.GetCategoryList(Constants.BASE_CATEGORY).FirstOrDefault(cat => cat.ID.Equals(((int)category).ToString(Constants.PADDING_CATEGORYID)))?.Name ?? "";

                if (subcategory != null)
                {
                    data.SubcategoryName = CacheHelper.GetCategoryList(((int)category).ToString(Constants.PADDING_CATEGORYID)).FirstOrDefault(cat => cat.ID.Equals(((int)subcategory).ToString(Constants.PADDING_CATEGORYID)))?.Name ?? "";
                }
            }

            data.Categories = CacheHelper.GetCategoryList(Constants.BASE_CATEGORY);


            if (!string.IsNullOrEmpty(errorCode))
            {
                ViewData["errorCode"] = errorCode;
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult Add(TechnologyEntity technology, string StatusRemarks, bool isSubcategoryInQuery)
        {
            int errorCode = 0;
            string nextID = "";

            try
            {
                nextID = SequenceHelper.GetNextSequence(Constants.ID, Constants.TABLE_TECHNOLOGY, Constants.PADDING_TECHNOLOGYID);

                technology.RowKey = technology.RowKey + "_" + nextID;

                technology.Description = technology.Description ?? "";

                technology.Tag = InsertTag(technology.NewTag, technology.Tag);

                DuplicateCheckEntity duplicateCheck = new DuplicateCheckEntity(technology.PartitionKey, technology.SubcategoryID + "_" + technology.NameHash);

                TechnologyStatusEntity technologyStatus = GetTechnologyStatusEntity(technology.PartitionKey, nextID, technology.Status, StatusRemarks);

                TableStorageHelper.InsertBatchAsync(Constants.TABLE_TECHNOLOGY, duplicateCheck, technology, technologyStatus).Wait();
            }
            catch (Exception ex)
            {
                nextID = "";
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new {errorCode = errorCode, category = technology.PartitionKey, subcategory = ((isSubcategoryInQuery) ? technology.SubcategoryID : ""), technology = nextID });
        }

        [HttpPost]
        public IActionResult Edit(TechnologyEntity technology, string OriginalNameHash, bool isSubcategoryInQuery)
        {
            int errorCode = 0;

            try
            {
                technology.Description = technology.Description ?? "";

                technology.Tag = InsertTag(technology.NewTag, technology.Tag);

                if (OriginalNameHash.Equals(technology.NameHash))
                {
                    TableStorageHelper.MergeAsync(Constants.TABLE_TECHNOLOGY, technology).Wait();
                }
                else
                {
                    DuplicateCheckEntity duplicateCheckOriginalDelete = new DuplicateCheckEntity(technology.PartitionKey, technology.SubcategoryID + "_" + OriginalNameHash);
                    duplicateCheckOriginalDelete.ETag = "*";

                    DuplicateCheckEntity duplicateCheckNewAdd = new DuplicateCheckEntity(technology.PartitionKey, technology.SubcategoryID + "_" + technology.NameHash);

                    TableStorageHelper.BatchAsync(Constants.TABLE_TECHNOLOGY, new DuplicateCheckEntity[] { duplicateCheckOriginalDelete }, new DuplicateCheckEntity[] { duplicateCheckNewAdd }, new TechnologyEntity[] { technology }).Wait();
                }
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { errorCode = errorCode, category = technology.PartitionKey, subcategory = ((isSubcategoryInQuery) ? technology.SubcategoryID : ""), technology = technology.ID });
        }

        [HttpPost]
        public IActionResult UpdateStatus(TechnologyEntity technology, string StatusRemarks, bool isSubcategoryInQuery)
        {
            int errorCode = 0;

            try
            {
                technology.Description = technology.Description ?? "";

                TechnologyStatusEntity technologyStatus = GetTechnologyStatusEntity(technology.PartitionKey, technology.ID, technology.Status, StatusRemarks);

                TableStorageHelper.BatchAsync(Constants.TABLE_TECHNOLOGY, null, new TechnologyStatusEntity[] { technologyStatus }, new TechnologyEntity[] { technology }).Wait();
            }
            catch (Exception ex)
            {
                if (!TableStorageHelper.IsStorageException(ex, out errorCode))
                {
                    errorCode = Constants.ERROR_CODE_COMMON;
                }
            }

            return RedirectToAction("Index", new { errorCode = errorCode, category = technology.PartitionKey, subcategory = ((isSubcategoryInQuery) ? technology.SubcategoryID : ""), technology = technology.ID });
        }

        public IActionResult StatusHistory(string PartitionKey, string RowKey)
        {
            ViewData["divID"] = (PartitionKey ?? "") + "_" + (RowKey ?? "");

            string technologyID = (RowKey ?? "T_ASDF_-12345").Split('_')[2];

            string value1 = "S_" + technologyID;

            string value2 = "S_" + (int.Parse(technologyID) + 1).ToString(Constants.PADDING_TECHNOLOGYID);

            IEnumerable<TechnologyStatusEntity> statusList = TableStorageHelper.RetrieveByRangeAsync<TechnologyStatusEntity>(Constants.TABLE_TECHNOLOGY, PartitionKey ?? "", "ge", value1, "lt", value2).Result;


            return PartialView(statusList);
        }

        private TechnologyStatusEntity GetTechnologyStatusEntity(string partitionKey, string technologyID, int status, string statusRemarks)
        {
            string invertedTicks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);

            TechnologyStatusEntity entity = new POCO.TechnologyStatusEntity();

            entity.PartitionKey = partitionKey;

            entity.RowKey = "S_" + technologyID + "_" + invertedTicks;

            entity.Status = status;

            entity.Remarks = statusRemarks ?? "";

            return entity;
        }

        private string InsertTag(string newTag, string currentTag)
        {
            currentTag = currentTag?.Trim() ?? "";

            bool newTagAdded = false;

            int nextID = -1;
            bool nextIDUsed = true;

            if (!string.IsNullOrEmpty(newTag?.Trim()) && newTag.Trim() != "|")
            {
                string[] nt = newTag.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string text in nt)
                {
                    if (!string.IsNullOrEmpty(text?.Trim()))
                    {
                        TagEntity tag = CacheHelper.GetTagList().FirstOrDefault(te => te.RowKey.Trim().ToLower() == text.Trim().ToLower());

                        if (tag == null)
                        {
                            if (nextIDUsed)
                            {
                                nextID = int.Parse(SequenceHelper.GetNextSequence("ID", Constants.TABLE_TAG, null));
                            }
                            tag = new POCO.TagEntity("Tag", text.Trim()) { ID = nextID };

                            try
                            {
                                TableStorageHelper.InsertAsync(Constants.TABLE_TAG, tag).Wait();
                                nextIDUsed = true;
                                newTagAdded = true;
                            }
                            catch(Exception ex)
                            {
                                nextIDUsed = false;
                                CacheHelper.ClearTagListCache();
                                tag = CacheHelper.GetTagList().FirstOrDefault(te => te.RowKey.Trim().ToLower() == text.Trim().ToLower());
                            }
                        }

                        if (tag != null)
                        {
                            currentTag = currentTag + (currentTag.Trim().Length == 0 ? "|" : "") + tag.ID + "|";
                        }
                    }
                }

                if (newTagAdded)
                {
                    CacheHelper.ClearTagListCache();
                }
            }


            return currentTag;
        }
    }
}
