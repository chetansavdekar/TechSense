using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class TechnologyViewModel
    {
        public IEnumerable<TechnologyEntity> Technologies;

        public IEnumerable<CategoryEntity> Categories;

        public IEnumerable<CategoryEntity> Subcategories;

        public IEnumerable<TagEntity> TagList;

        public string CategoryID;

        public string SubcategoryID;

        public string TechnologyID;

        public string CategoryName;

        public string SubcategoryName;

        public string TechnologyName;

        public SelectListGroup Active;

        public SelectListGroup Inactive;

        public string TechnologyDropDownID
        {
            get
            {
                string value = "";

                value = (CategoryID ?? "") + "_T_" + (SubcategoryID ?? "") + "_" + (TechnologyID ?? "");

                return (value.Length == 4 ? "" : value);
            }
        }
    }
}
