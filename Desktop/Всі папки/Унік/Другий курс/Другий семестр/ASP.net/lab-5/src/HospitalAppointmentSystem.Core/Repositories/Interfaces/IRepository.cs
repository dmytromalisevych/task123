using System.Linq.Expressions;
using HospitalAppointmentSystem.Core.Models.Base;

namespace HospitalAppointmentSystem.Core.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        
        Task<T> AddWithUserAsync(T entity, string password);
        Task UpdateWithUserAsync(T entity);
        Task DeleteWithUserAsync(int id);
        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(IEnumerable<int> ids);
        
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        
        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties);
        
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(int id);
        
        Task<T> AddAndSaveAsync(T entity);
        Task UpdateAndSaveAsync(T entity);
        Task DeleteAndSaveAsync(int id);
    }
}