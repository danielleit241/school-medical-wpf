using SchoolMedicalWpf.Dal.Entities;

public class RoleService
{
    private readonly RoleRepository _roleRepository;

    public RoleService(RoleRepository roleRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<List<Role>> GetAllRoles()
    {
        try
        {
            // Kiểm tra nếu các Role có trong cơ sở dữ liệu
            var roles = await _roleRepository.GetAllRoles();
            if (roles == null || !roles.Any())
            {
                // Nếu không có role nào, có thể trả về một danh sách trống hoặc throw exception
                throw new Exception("No roles found in the database.");
            }
            return roles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching roles: {ex.Message}");
            throw;  // Propagate the error to higher layers
        }
    }

    public async Task<Role?> GetRoleById(int roleId)
    {
        if (roleId <= 0)
            throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));

        return await _roleRepository.GetRoleById(roleId);
    }
}
