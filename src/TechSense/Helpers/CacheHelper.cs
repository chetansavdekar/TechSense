using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSense.POCO;

namespace TechSense.Helpers
{
    public static class CacheHelper
    {
        private static IList<CategoryEntity> _categoryList = null;
        private static Object _syncLockCategoryList = new Object();


        private static IList<TagEntity> _tagList = null;
        private static Object _syncLockTagList = new Object();

        private static IList<UserEntity> _users = null;
        private static Object _syncLockUsers = new object();

        public static IEnumerable<CategoryEntity> GetCategoryList(string category = null)
        {
            if (_categoryList == null)
            {
                lock(_syncLockCategoryList)
                {
                    if (_categoryList == null)
                    {
                        _categoryList = TableStorageHelper.RetrieveAllAsync<CategoryEntity>(Constants.TABLE_CATEGORY).Result;

                        if (category != null)
                            return _categoryList.Where(c => c.PartitionKey == category); //returning here itself b/c of UpdateCategoryListCache method
                        else
                            return _categoryList;
                    }
                }
            }

            if (category != null)
                return _categoryList.Where(c => c.PartitionKey == category);
            else
                return _categoryList;
        }

        public static void ClearCategoryListCache()
        {
            lock (_syncLockCategoryList)
            {
                _categoryList = null;
            }
        }


        public static IEnumerable<TagEntity> GetTagList()
        {
            if (_tagList == null)
            {
                lock (_syncLockTagList)
                {
                    if (_tagList == null)
                    {
                        _tagList = TableStorageHelper.RetrieveAllAsync<TagEntity>(Constants.TABLE_TAG).Result;
                        return _tagList; //returning here itself b/c of UpdateTagListCache method
                    }
                }
            }

            return _tagList;
        }

        public static void ClearTagListCache()
        {
            lock (_syncLockTagList)
            {
                _tagList = null;
            }
        }

        public static IEnumerable<UserEntity> GetUsers()
        {
            if (_users == null)
            {
                lock (_syncLockUsers)
                {
                    if (_users == null)
                    {
                        _users = TableStorageHelper.RetrieveAllAsync<UserEntity>(Constants.TABLE_USER).Result;
                        return _users; //returning here itself b/c of UpdateTagListCache method
                    }
                }
            }

            return _users;
        }

        public static void ClearUsersCache()
        {
            lock (_syncLockUsers)
            {
                _users = null;
            }
        }
    }
}
