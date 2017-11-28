using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class DuplicateCheckEntity : TableEntity
    {
        public DuplicateCheckEntity()
        {
        }

        public DuplicateCheckEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
