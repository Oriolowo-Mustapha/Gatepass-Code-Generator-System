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

        public async Task<Guid> GetDeptIdBydeptCodeAsync(string deptCode, CancellationToken cancellationToken = default)
        {
            var dept = await _dbSet.FirstOrDefaultAsync(d => d.DepartmentCode.ToUpper() == deptCode.ToUpper(), cancellationToken);

            if (dept == null) { 

                throw new KeyNotFoundException($"Department with name '{deptCode}' was not foumd");
            }
            return dept.Id;
        }
    }
}
