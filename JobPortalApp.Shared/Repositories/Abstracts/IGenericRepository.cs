using JobPortalApp.Shared.Entities;
using System.Linq.Expressions;

namespace JobPortalApp.Shared.Repositories.Abstracts;

public interface IGenericRepository<TEntity, in TId>
    where TEntity : BaseEntity<TId>, new()
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TId id);
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>>? predicate = null);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}