using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class RoleRepository
    {
        private readonly IDbContextFactory<SchoolmedicalWpfContext> _contextFactory;

        public RoleRepository(IDbContextFactory<SchoolmedicalWpfContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Roles.ToListAsync().ConfigureAwait(false);
        }

        public async Task<Role?> GetRoleById(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));

            using var context = _contextFactory.CreateDbContext();
            return await context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }
    }
}
