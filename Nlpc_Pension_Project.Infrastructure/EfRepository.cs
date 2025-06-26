
using Microsoft.EntityFrameworkCore;
using Nlpc_Pension_Project.Domain;
using Nlpc_Pension_Project.Domain.Entities;

namespace Nlpc_Pension_Project.Infrastructure;

// IRepository.cs


// EfRepository.cs
public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _dbContext;

    public EfRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    //public async Task<IReadOnlyList<T>> ListAllAsync()
    //{
    //    return await _dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
    //}

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _dbContext.Set<T>()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt) 
            .ToListAsync();
    }



    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(entity);

    }
    public async Task<T> GetMemberByIdAsync(int id)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (typeof(T) == typeof(Member))
        {
            query = query
                .Include(nameof(Member.Contributions))
                .Include(nameof(Member.Benefits));
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<IReadOnlyList<T>> ListAllMemberAsync()
    {
        IQueryable<T> query = _dbContext.Set<T>().Where(e => !e.IsDeleted);

        if (typeof(T) == typeof(Member))
        {
            query = query
                .Include(nameof(Member.Contributions))
                .Include(nameof(Member.Benefits));
        }

        return await query.ToListAsync();
    }
}
