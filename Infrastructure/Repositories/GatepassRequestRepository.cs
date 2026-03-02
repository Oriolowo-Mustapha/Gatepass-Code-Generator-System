using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class GatepassRequestRepository : GenericRepository<GatepassRequest>, IGatepassRequestRepository
{
    public GatepassRequestRepository(ApplicationDbContext context) : base(context)
    {
    }
}
