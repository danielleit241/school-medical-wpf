using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal;

public class UserRepository
{
    private readonly DbContextOptions<SchoolmedicalWpfContext> _options;

    public UserRepository(DbContextOptions<SchoolmedicalWpfContext> options)
    {
        _options = options;
    }

    public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUser(User user)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Users.Include(u => u.Role).ToListAsync();
    }

    public async Task<List<User>> GetUsersByRoleId(int roleId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Users
            .Where(u => u.RoleId == roleId)
            .ToListAsync();
    }

    public async Task AddUser(User user)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user != null)
        {
            user.Status = false;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
