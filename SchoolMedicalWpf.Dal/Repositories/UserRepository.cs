using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal;

public class UserRepository
{
    private readonly SchoolmedicalWpfContext _context;

    // Sử dụng DbContextFactory thay vì DbContext
    public UserRepository(SchoolmedicalWpfContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.Include(u => u.Role).ToListAsync();
    }

    public async Task<List<User>> GetUsersByRoleId(int roleId)
    {
        return await _context.Users
            .Where(u => u.RoleId == roleId)
            .ToListAsync();
    }

    public async Task AddUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user != null)
        {
            user.Status = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
