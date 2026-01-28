using Infotecs2026.Share.Models;

namespace Infotecs2026.Share.Application.Interfaces;

public interface IValueRepository
{
    Task AddAndSaveAsync(Value value);
    Task AddAsync(Value value);
    void Update(Value value);
    Task SaveAsync();
    Task<List<Value>> GetByNameAsync(string fileName);

    void RemoveRange(List<Value> values);
}
