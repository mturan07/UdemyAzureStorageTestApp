//using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : ITableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cloudTable;

        public TableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionStrigs.AzureConnectionString);
            _cloudTableClient = storageAccount.CreateCloudTableClient();
            _cloudTable = _cloudTableClient.GetTableReference(typeof(TEntity).Name);
            _cloudTable.CreateIfNotExistsAsync().Wait();
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var operation = TableOperation.InsertOrMerge(entity);
            var action = await _cloudTable.ExecuteAsync(operation);
            return (TEntity)action.Result;
        }

        public Task Delete(string rowKey, string partitionKey)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Get(string rowKey, string partitionKey)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _cloudTable.CreateQuery<TEntity>().AsQueriable();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
