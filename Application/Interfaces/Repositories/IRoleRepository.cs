using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Guid> GetIdByRoleNameAsync(string Name, CancellationToken cancellationToken = default);
    }
}
