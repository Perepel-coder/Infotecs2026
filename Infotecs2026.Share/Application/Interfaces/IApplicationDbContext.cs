using Infotecs2026.Share.Models;
using Microsoft.EntityFrameworkCore;

namespace Infotecs2026.Share.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Value> Values { get; set; }
    DbSet<Result> Results { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
