using SchoolMedicalWpf.Dal.Entities;

public class RoleService
{
    private readonly RoleRepository _roleRepository;

    public RoleService(RoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<Role>> GetAllRoles()
    {
        var roles = await _roleRepository.GetAllRoles();
        if (roles == null || roles.Count == 0)
            throw new InvalidOperationException("No roles found in the database.");
        return roles;
    }

    public async Task<Role?> GetRoleById(int roleId)
    {
        if (roleId <= 0)
            throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));

        return await _roleRepository.GetRoleById(roleId);
    }
}
