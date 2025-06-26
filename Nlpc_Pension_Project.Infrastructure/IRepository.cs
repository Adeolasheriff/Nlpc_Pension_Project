using Nlpc_Pension_Project.Domain;

namespace Nlpc_Pension_Project.Infrastructure;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IReadOnlyList<T>> ListAllMemberAsync();
    Task<T> GetMemberByIdAsync(int id);

}