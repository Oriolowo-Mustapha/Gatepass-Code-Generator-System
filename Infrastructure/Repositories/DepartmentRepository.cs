using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartementRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Guid> GetDeptIdBydeptNameAsync(string deptName, CancellationToken cancellationToken = default)
        {
            var dept = await _dbSet.FirstOrDefaultAsync(d => d.DepartmentName == deptName, cancellationToken);

            if (dept == null) { 

                throw new KeyNotFoundException($"Department with name '{deptName}' was not foumd");
            }
            return dept.Id;
        }
    }
}
