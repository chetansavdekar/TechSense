using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense
{
    public class Constants
    {
        public const string TABLE_CATEGORY = "TBCategory";
        public const string TABLE_SEQUENCE = "TBSequence";
        public const string TABLE_TECHNOLOGY = "TBTechnology";
        public const string TABLE_TAG = "TBTag";
        public const string TABLE_USER = "TBUser";

        public const string BASE_CATEGORY = "Category";
        public const string ID = "ID";
        public const string SORT = "Sort";

        public const string PADDING_SORT = "D4";
        public const string PADDING_CATEGORYID = "D3";
        public const string PADDING_TECHNOLOGYID = "D4";

        public const int ERROR_CODE_COMMON = 1000;
        public const string ERROR_COMMON = "Error occurred. Please try again.";

        public const int ERROR_CODE_ACCESS_DENIED = 1001;
        public const string ERROR_ACCESS_DENIED = "Access Denied";

        public const int ERROR_CODE_PRECONDITION_FAILED = 412;
        public const string ERROR_PRECONDITION_FAILED = "Operation failed because another user has updated or deleted the record. Your changes have been lost. Please review their changes before trying again.";

        public const int ERROR_CODE_ENTITY_ALREADY_EXISTS = 409;
        public const string ERROR_ENTITY_ALREADY_EXISTS = "The specified name already exists.";
    }
}
