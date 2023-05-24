using Template.Core.Models.Components;

namespace Template.Core.Interfaces.Repositories.Shared;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindAsync(Guid id);
    public Task<IEnumerable<T>> FindAsync(params Guid[] ids);
    public Task<IEnumerable<T>> FindAllAsync();
    public Task<T> CreateAsync(T t);
    public Task<T> UpdateAsync(T t);
    public Task<bool> DeleteAsync(Guid id);
}