using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IDepartementRepository : IGenericRepository<Department>
    {
        Task<Guid> GetDeptIdBydeptCodeAsync(string deptcode, CancellationToken  cancellationToken= default);    }
}
