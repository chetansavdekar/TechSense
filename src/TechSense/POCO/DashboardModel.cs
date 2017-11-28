using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class DashboardModel
    {
        public IDictionary<string, IEnumerable<TechnologyEntity>> Technologies;

        public IEnumerable<CategoryEntity> Categories;

        public IDictionary<string, IEnumerable<CategoryEntity>> Subcategories;

        public IEnumerable<TagEntity> TagList;

        public string CurrentCategoryID;

        public int Priority;

        public string Top;

        public string Tag;

        public HashSet<string> StyleIDForTags;
    }
}
