using Application.Interfaces.Repositories;

namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IDepartementRepository Departements { get; }
    IGatepassRepository Gatepasses { get; }
    IGatepassRequestRepository GatepassRequests { get; }
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
