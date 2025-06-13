using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal;

public class RoleRepository
{
    private SchoolmedicalWpfContext _context;

    public async Task<List<Role>> GetAllRoles()
    {
        _context = new();
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleById(int roleId)
    {
        _context = new();
        return await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }
}
