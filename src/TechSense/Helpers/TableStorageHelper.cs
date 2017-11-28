using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.Helpers
{
    public class TableStorageHelper
    {
        public static async Task<CloudTable> GetTableReferenceAsync(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configurations.StorageConfiguration.StorageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            return table;
        }

        public static async Task<IList<T>> RetrieveAllAsync<T>(string tableName) where T : ITableEntity, new()
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableQuery<T> query = new TableQuery<T>();

            return await ExecuteRetrieveAsync<T>(table, query);
        }

        public static async Task<IList<T>> RetrieveByPartitionKeyAsync<T>(string tableName, string partitionKey) where T : ITableEntity, new()
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            return await ExecuteRetrieveAsync<T>(table, query);
        }

        public static async Task<IList<T>> RetrieveByRangeAsync<T>(string tableName, string partitionKey, string operator1, string value1, string operator2 = null, string value2 = null) where T : ITableEntity, new()
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);


            string condition = TableQuery.CombineFilters(
                                                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                                                TableOperators.And,
                                                TableQuery.GenerateFilterCondition("RowKey", operator1, value1)
                                                );

            if (!string.IsNullOrEmpty(operator2) || !string.IsNullOrEmpty(value2))
            {
                condition = TableQuery.CombineFilters(
                                                condition,
                                                TableOperators.And,
                                                TableQuery.GenerateFilterCondition("RowKey", operator2, value2)
                                                );
            }

            
            TableQuery<T> query = new TableQuery<T>().Where(condition);

            return await ExecuteRetrieveAsync<T>(table, query);
        }

        public static async Task<T> RetrieveAsync<T>(string tableName, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            TableResult result = await table.ExecuteAsync(retrieveOperation);

            return (T)result.Result;
        }

        private static async Task<IList<T>> ExecuteRetrieveAsync<T>(CloudTable table, TableQuery<T> query) where T : ITableEntity, new()
        {
            TableContinuationToken continuationToken = null;

            List<T> list = new List<T>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                list.AddRange(queryResult);

                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken != null);

            return list;
        }

        public static async Task InsertOrMergeAsync(string tableName, ITableEntity entity)
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableOperation operation = TableOperation.InsertOrMerge(entity);

            await table.ExecuteAsync(operation);
        }

        public static async Task InsertAsync(string tableName, ITableEntity entity)
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableOperation operation = TableOperation.Insert(entity);

            await table.ExecuteAsync(operation);
        }

        public static async Task MergeAsync(string tableName, ITableEntity entity)
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableOperation operation = TableOperation.Merge(entity);

            await table.ExecuteAsync(operation);
        }

        public static async Task InsertBatchAsync(string tableName, params ITableEntity[] insertEntities)
        {
            if (((insertEntities?.Length ?? 0) > 0))
            {
                CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

                TableBatchOperation batchOperation = new TableBatchOperation();

                for (int i = 0; i < insertEntities.Length; i++)
                {
                    if (insertEntities[i] != null)
                    {
                        batchOperation.Insert(insertEntities[i]);
                    }
                }

                await table.ExecuteBatchAsync(batchOperation);
            }
        }

        public static async Task BatchAsync(string tableName, ITableEntity[] deleteEntities, ITableEntity[] insertEntities, ITableEntity[] mergeEntities)
        {
            if (((mergeEntities?.Length ?? 0) > 0) || ((insertEntities?.Length ?? 0) > 0) || ((deleteEntities?.Length ?? 0) > 0))
            {
                CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

                TableBatchOperation batchOperation = new TableBatchOperation();

                if (deleteEntities != null)
                {
                    for (int i = 0; i < deleteEntities.Length; i++)
                    {
                        if (deleteEntities[i] != null)
                        {
                            batchOperation.Delete(deleteEntities[i]);
                        }
                    }
                }

                if (insertEntities != null)
                {
                    for (int i = 0; i < insertEntities.Length; i++)
                    {
                        if (insertEntities[i] != null)
                        {
                            batchOperation.Insert(insertEntities[i]);
                        }
                    }
                }

                if (mergeEntities != null)
                {
                    for (int i = 0; i < mergeEntities.Length; i++)
                    {
                        if (mergeEntities[i] != null)
                        {
                            batchOperation.Merge(mergeEntities[i]);
                        }
                    }
                }

                await table.ExecuteBatchAsync(batchOperation);
            }
        }

        public static async Task DeleteAsync<T>(string tableName, T entity) where T : ITableEntity, new()
        {
            CloudTable table = await TableStorageHelper.GetTableReferenceAsync(tableName);

            TableOperation operation = TableOperation.Delete(entity);

            await table.ExecuteAsync(operation);
        }

        public static bool IsStorageException(Exception ex, out int errorCode)
        {
            StorageException exStorage = ex.GetBaseException() as StorageException;

            if (exStorage != null)
            {
                errorCode = exStorage.RequestInformation.HttpStatusCode;
                return true;
            }
            else
            {
                errorCode = 0;
                return false;
            }
        }
    }
}
