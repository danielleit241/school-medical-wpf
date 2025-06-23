using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;

public class RoleRepository
{
    private readonly SchoolmedicalWpfContext _context;

    public RoleRepository(SchoolmedicalWpfContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleById(int roleId)
    {
        return await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }
}
