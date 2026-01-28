using Infotecs2026.Share.Application.Interfaces;
using Infotecs2026.Share.Models;
using Microsoft.EntityFrameworkCore;

namespace Infotecs2026.Share.Application.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly IApplicationDbContext _context;

    public ResultRepository(IApplicationDbContext context) => _context = context;

    public async Task AddAndSaveAsync(Result result)
    {
        await _context.Results.AddAsync(result);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Result result) => await _context.Results.AddAsync(result);

    public async Task<List<Result>> GetByNameAsync(string fileName) =>
        await _context.Results.Where(v => v.FileName == fileName).ToListAsync();

    public void Update(Result result) => _context.Results.Update(result);

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public void RemoveRange(List<Result> results) => _context.Results.RemoveRange(results);
}
