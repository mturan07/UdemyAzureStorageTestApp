using System.Linq.Expressions;

namespace AzureStorageLibrary
{
    public interface INoSqlStorage<ITableEntity>
    {
        Task<ITableEntity> Add(ITableEntity entity);
        Task<ITableEntity> Get(string rowKey, string partitionKey);
        Task Delete(string rowKey, string partitionKey);
        Task<ITableEntity> Update(ITableEntity entity);
        IQueryable<ITableEntity> GetAll();
        IQueryable<ITableEntity> Query(Expression<Func<ITableEntity, bool>> query);
    }
}
