using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class SequenceEntity : TableEntity
    {
        private int _nextSequence;

        public SequenceEntity()
        {
        }

        public SequenceEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public int NextSequence
        {
            get
            {
                return _nextSequence;
            }
            set
            {
                _nextSequence = value;
            }
        }
    }
}
