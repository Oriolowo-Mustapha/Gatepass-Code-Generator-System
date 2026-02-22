using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GatepassRepository : GenericRepository<Gatepass>, IGatepassRepository
{
    public GatepassRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Gatepass?> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.GatepassRequest)
                .ThenInclude(gr => gr.Visitor)
            .FirstOrDefaultAsync(g => g.UniqueCode == uniqueCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Gatepass>> GetActiveGatepassesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(g => g.IsActive && !g.IsRevoked && g.ValidUntil > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
