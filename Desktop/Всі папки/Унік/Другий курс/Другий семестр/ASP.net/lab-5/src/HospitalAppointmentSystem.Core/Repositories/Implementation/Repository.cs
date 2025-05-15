using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HospitalAppointmentSystem.Core.Data;
using HospitalAppointmentSystem.Core.Models;
using HospitalAppointmentSystem.Core.Models.Base;
using HospitalAppointmentSystem.Core.Repositories.Interfaces;

namespace HospitalAppointmentSystem.Core.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;
        protected readonly UserManager<IdentityUser>? UserManager;

        public Repository(AppDbContext context, UserManager<IdentityUser>? userManager = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = Context.Set<T>();
            UserManager = userManager;
        }

        protected virtual async Task<IdentityResult> HandleUserOperationAsync(BaseEntity entity, Func<IdentityUser, Task<IdentityResult>> operation)
        {
            if (UserManager == null)
                throw new InvalidOperationException("UserManager is not initialized");

            string? userId = entity switch
            {
                Doctor doctor => doctor.UserId,
                Patient patient => patient.UserId,
                _ => null
            };

            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("UserId is not set");

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User not found with id: {userId}");

            return await operation(user);
        }
        public virtual async Task<T> AddAndSaveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                await DbSet.AddAsync(entity);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entity;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task UpdateAndSaveAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                entity.UpdatedAt = DateTime.UtcNow;
                Context.Entry(entity).State = EntityState.Modified;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task DeleteAndSaveAsync(int id)
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    DbSet.Remove(entity);
                    await Context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public virtual async Task<T> AddWithUserAsync(T entity, string password)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty", nameof(password));
            if (UserManager == null) throw new InvalidOperationException("UserManager is not initialized");

            string email = entity switch
            {
                Doctor doctor => doctor.Email,
                Patient patient => patient.Email,
                _ => throw new InvalidOperationException($"Entity type {typeof(T).Name} is not supported for user creation")
            };

            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await UserManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                var roleName = entity switch
                {
                    Doctor => "Doctor",
                    Patient => "Patient",
                    _ => throw new InvalidOperationException($"Unexpected entity type: {entity.GetType().Name}")
                };

                await UserManager.AddToRoleAsync(user, roleName);

                switch (entity)
                {
                    case Doctor doctor:
                        doctor.UserId = user.Id;
                        break;
                    case Patient patient:
                        patient.UserId = user.Id;
                        break;
                }

                entity.CreatedAt = DateTime.UtcNow;
                await DbSet.AddAsync(entity);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return entity;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task UpdateWithUserAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (UserManager == null) throw new InvalidOperationException("UserManager is not initialized");

            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                string email = entity switch
                {
                    Doctor doctor => doctor.Email,
                    Patient patient => patient.Email,
                    _ => throw new InvalidOperationException($"Entity type {typeof(T).Name} is not supported for user update")
                };

                await HandleUserOperationAsync(entity, async user =>
                {
                    user.Email = email;
                    user.UserName = email;
                    return await UserManager.UpdateAsync(user);
                });

                entity.UpdatedAt = DateTime.UtcNow;
                Context.Entry(entity).State = EntityState.Modified;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public virtual async Task DeleteWithUserAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;

            await using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                await HandleUserOperationAsync(entity, async user => await UserManager!.DeleteAsync(user));

                DbSet.Remove(entity);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            entity.CreatedAt = DateTime.UtcNow;
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.UpdatedAt = DateTime.UtcNow;
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
                await Context.SaveChangesAsync();
            }
        }
        
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await DbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await DbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await DbSet.AnyAsync(predicate);
        }
        
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            
            var entitiesList = entities.ToList();
            var utcNow = DateTime.UtcNow;
            foreach (var entity in entitiesList)
            {
                entity.CreatedAt = utcNow;
            }
            
            await DbSet.AddRangeAsync(entitiesList);
            await Context.SaveChangesAsync();
            return entitiesList;
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            
            var utcNow = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                entity.UpdatedAt = utcNow;
                Context.Entry(entity).State = EntityState.Modified;
            }
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<int> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            
            var entities = await DbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
            if (entities.Any())
            {
                DbSet.RemoveRange(entities);
                await Context.SaveChangesAsync();
            }
        }
        
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            if (pageNumber < 1) throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            var query = DbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            if (includeProperties == null) throw new ArgumentNullException(nameof(includeProperties));

            var query = includeProperties.Aggregate(
                DbSet.AsQueryable(),
                (current, includeProperty) => current.Include(includeProperty));

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            if (includeProperties == null) throw new ArgumentNullException(nameof(includeProperties));

            var query = includeProperties.Aggregate(
                DbSet.AsQueryable(),
                (current, includeProperty) => current.Include(includeProperty));

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null 
                ? await DbSet.CountAsync() 
                : await DbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await DbSet.AnyAsync(e => e.Id == id);
        }
    }
}