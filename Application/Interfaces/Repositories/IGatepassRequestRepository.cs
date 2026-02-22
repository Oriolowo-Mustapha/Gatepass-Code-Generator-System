using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IGatepassRequestRepository : IGenericRepository<GatepassRequest>
{
    Task<IReadOnlyList<GatepassRequest>> GetPendingRequestsForHostAsync(Guid hostId, CancellationToken cancellationToken = default);
}
