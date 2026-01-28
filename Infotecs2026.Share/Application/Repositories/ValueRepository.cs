using Infotecs2026.Share.Application.Interfaces;
using Infotecs2026.Share.Models;
using Microsoft.EntityFrameworkCore;

namespace Infotecs2026.Share.Application.Repositories;

public class ValueRepository : IValueRepository
{
    private readonly IApplicationDbContext _context;

    public ValueRepository(IApplicationDbContext context) => _context = context;

    public async Task AddAndSaveAsync(Value value)
    {
        await _context.Values.AddAsync(value);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Value value) => await _context.Values.AddAsync(value);

    public async Task<List<Value>> GetByNameAsync(string fileName) =>
        await _context.Values.Where(v => v.FileName == fileName).ToListAsync();

    public void Update(Value value) => _context.Values.Update(value);

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public void RemoveRange(List<Value> values) => _context.Values.RemoveRange(values);
}
