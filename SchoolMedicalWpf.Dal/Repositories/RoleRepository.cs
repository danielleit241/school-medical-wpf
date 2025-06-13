using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal;

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
        return await context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleById(int roleId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }
}
