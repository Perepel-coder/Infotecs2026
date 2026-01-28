using Infotecs2026.Share.Models;

namespace Infotecs2026.Share.Application.Interfaces;

public interface IResultRepository
{
    Task AddAndSaveAsync(Result results);
    Task AddAsync(Result results);
    void Update(Result results);
    Task SaveAsync();
    Task<List<Result>> GetByNameAsync(string fileName);

    void RemoveRange(List<Result> results);
}
