using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IGatepassRepository : IGenericRepository<Gatepass>
{
    Task<Gatepass?> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Gatepass>> GetActiveGatepassesAsync(CancellationToken cancellationToken = default);
}
