using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GatepassRequestRepository : GenericRepository<GatepassRequest>, IGatepassRequestRepository
{
    public GatepassRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GatepassRequest>> GetPendingRequestsForHostAsync(Guid hostId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(gr => gr.HostUserId == hostId && gr.ApprovalStatus == ApprovalStatus.Pending)
            .Include(gr => gr.Visitor)
            .OrderByDescending(gr => gr.RequestDate)
            .ToListAsync(cancellationToken);
    }
}
