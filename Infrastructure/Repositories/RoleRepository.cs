using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Guid> GetIdByRoleNameAsync(string Name, CancellationToken cancellationToken = default)
        {
            var role = await _dbSet.FirstOrDefaultAsync(r => Name == r.RoleName, cancellationToken);
    
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with name '{Name}' was not found.");
            }
    
            return role.Id;
        }
    }
}
