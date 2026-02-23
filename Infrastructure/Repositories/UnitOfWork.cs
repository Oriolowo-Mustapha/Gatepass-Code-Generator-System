using System.Collections;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Hashtable _repositories = new();

    public IUserRepository Users { get; }
    public IGatepassRepository Gatepasses { get; }
    public IGatepassRequestRepository GatepassRequests { get; }

    public IRoleRepository Roles { get; }

    public IDepartementRepository Departements { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository users,
        IGatepassRepository gatepasses,
        IGatepassRequestRepository gatepassRequests)
    {
        _context = context;
        Users = users;
        Gatepasses = gatepasses;
        GatepassRequests = gatepassRequests;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var typeName = typeof(TEntity).Name;

        if (_repositories.ContainsKey(typeName))
            return (IGenericRepository<TEntity>)_repositories[typeName]!;

        var repository = new GenericRepository<TEntity>(_context);
        _repositories[typeName] = repository;
        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
