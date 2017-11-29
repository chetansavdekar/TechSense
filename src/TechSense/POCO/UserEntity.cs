using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class UserEntity : TableEntity
    {
        private string _password;

        private string _accessLevel;
        public UserEntity()
        {
        }

        public UserEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        [IgnoreProperty]
        public string UsernameHash
        {
            get
            {
                return RowKey?.Trim()?.ToLower() ?? "";
            }
        }

        public string AccessLevel
        {
            get
            {
                return _accessLevel;
            }
            set
            {
                _accessLevel = value;
            }
        }
    }
}
