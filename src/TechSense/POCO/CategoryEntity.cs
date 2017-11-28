using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class CategoryEntity : TableEntity
    {
        private string _name;
        private int _visibility;

        public CategoryEntity()
        {
        }

        public CategoryEntity(string category, string subcategory)
        {
            PartitionKey = category;
            RowKey = subcategory;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [IgnoreProperty]
        public string NameHash
        {
            get
            {
                return _name.Trim().ToLower().Replace(" ", "");
            }
        }

        public int Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
            }
        }

        [IgnoreProperty]
        public string ID
        {
            get
            {
                if (RowKey != null)
                    return RowKey.Split('_')[1];
                else
                    return "";
            }
        }

        [IgnoreProperty]
        public string SortNumber
        {
            get
            {
                if (RowKey != null)
                    return RowKey.Split('_')[0];
                else
                    return "-1";
            }
        }

        public void CopyTo(CategoryEntity target, bool copyKeys)
        {
            if (copyKeys)
            {
                target.PartitionKey = this.PartitionKey;
                target.RowKey = this.RowKey;
            }

            target.Name = this.Name;
            target.Visibility = this.Visibility;
        }
    }
}
