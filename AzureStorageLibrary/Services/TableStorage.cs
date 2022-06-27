using Microsoft.Azure.Cosmos.Table;
using System.Linq.Expressions;

namespace AzureStorageLibrary.Services
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : ITableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _cloudTable;

        public TableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionStrings.AzureConnectionString);
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

        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await Get(rowKey, partitionKey);
            var tableOperation = TableOperation.Delete(entity);
            await _cloudTable.ExecuteAsync(tableOperation);
        }

        public async Task<TEntity> Get(string rowKey, string partitionKey)
        {
            var tableOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var execute = await _cloudTable.ExecuteAsync(tableOperation);
            return (TEntity)execute.Result;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _cloudTable.CreateQuery<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return _cloudTable.CreateQuery<TEntity>().Where(query);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var tableOperation = TableOperation.Replace(entity);
            var execute = await _cloudTable.ExecuteAsync(tableOperation);
            return (TEntity)execute.Result;
        }
    }
}
