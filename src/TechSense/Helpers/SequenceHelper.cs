using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechSense.POCO;

namespace TechSense.Helpers
{
    public static class SequenceHelper
    {
        private static Object _syncLock = new Object();

        public static string GetNextSequence(string partitionKey, string rowKey, string padding)
        {
            lock (_syncLock)
            {
                int nextSequence = -1;

                SequenceEntity entity = TableStorageHelper.RetrieveAsync<SequenceEntity>(Constants.TABLE_SEQUENCE, partitionKey, rowKey).Result;

                if (entity == null)
                {
                    nextSequence = 1;
                    entity = new SequenceEntity(partitionKey, rowKey);
                }
                else
                {
                    nextSequence = entity.NextSequence;
                }

                entity.NextSequence = nextSequence + 1;

                TableStorageHelper.InsertOrMergeAsync(Constants.TABLE_SEQUENCE, entity).Wait();

                if (padding != null)
                {
                    return nextSequence.ToString(padding);
                }
                else
                {
                    return nextSequence.ToString();
                }
            }
        }
    }
}
