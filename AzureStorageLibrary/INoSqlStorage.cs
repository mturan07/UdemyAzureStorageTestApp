using System.Linq.Expressions;

namespace AzureStorageLibrary
{
    public interface INoSqlStorage<TableEntity>
    {
        Task<TableEntity> Add(TableEntity entity);
        Task<TableEntity> Get(string rowKey, string partitionKey);
        Task Delete(string rowKey, string partitionKey);
        Task<TableEntity> Update(TableEntity entity);
        IQueryable<TableEntity> GetAll();
        IQueryable<TableEntity> Query(Expression<Func<TableEntity, bool>> query);
    }
}
