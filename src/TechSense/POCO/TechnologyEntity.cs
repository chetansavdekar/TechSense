using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSense.Helpers;

namespace TechSense.POCO
{
    public class TechnologyEntity : TableEntity
    {
        private string _name;

        private string _tag;

        private string _newTag;

        private string _description;

        private int _priority;

        private int _status;

        private int _visibility;

        public TechnologyEntity()
        {
        }

        public TechnologyEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
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

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        [IgnoreProperty]
        public string NewTag
        {
            get
            {
                return _newTag;
            }
            set
            {
                _newTag = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        [IgnoreProperty]
        public string PriorityText
        {
            get
            {
                return ((Priority)_priority).ToString();
            }
        }

        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        [IgnoreProperty]
        public string StatusText
        {
            get
            {
                return ((Status)_status).ToString();
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
        public string VisibilityText
        {
            get
            {
                return ((Visibility)_visibility).ToString();
            }
        }

        [IgnoreProperty]
        public string Category
        {
            get
            {
                if (PartitionKey != null)
                {
                    return CacheHelper.GetCategoryList(Constants.BASE_CATEGORY).FirstOrDefault(category => category.ID.Equals(PartitionKey))?.Name ?? "";
                }
                else
                {
                    return "";
                }
            }
        }

        [IgnoreProperty]
        public string Subcategory
        {
            get
            {
                if (RowKey != null)
                {
                    return CacheHelper.GetCategoryList(PartitionKey).FirstOrDefault(category => category.ID.Equals(SubcategoryID))?.Name ?? "";
                }
                else
                {
                    return "";
                }
            }
        }

        [IgnoreProperty]
        public string SubcategoryID
        {
            get
            {
                if (RowKey != null && RowKey.Split('_').Length > 1)
                {
                    return RowKey.Split('_')[1]; //T_SUBCATEGORYID_TECHNOLOGYID
                }
                else
                {
                    return "";
                }
            }
        }

        [IgnoreProperty]
        public string ID
        {
            get
            {
                if (RowKey != null && RowKey.Split('_').Length > 2)
                {
                    return RowKey.Split('_')[2]; //T_SUBCATEGORYID_TECHNOLOGYID
                }
                else
                {
                    return "";
                }
            }
        }

        [IgnoreProperty]
        public string TagString
        {
            get
            {
                StringBuilder returnValue = new StringBuilder();

                if (!string.IsNullOrEmpty(_tag?.Trim()))
                {
                    IEnumerable<TagEntity> tagList = CacheHelper.GetTagList();

                    foreach (string tag in _tag.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrEmpty(tag?.Trim()))
                        {
                            string s = tagList.FirstOrDefault(t => t.ID == int.Parse(tag.Trim()))?.RowKey?.Trim();

                            if (!string.IsNullOrEmpty(s))
                            {
                                returnValue.Append((returnValue.Length > 0 ? ", " : "") + s);
                            }
                        }
                    }
                }

                return returnValue.ToString();
            }
        }

    }
}
