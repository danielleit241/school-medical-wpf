using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class UserRepository
    {
        private readonly IDbContextFactory<SchoolmedicalWpfContext> _contextFactory;
        private readonly RoleRepository _roleRepository;

        // Nhận IDbContextFactory từ DI
        public UserRepository(IDbContextFactory<SchoolmedicalWpfContext> contextFactory, RoleRepository roleRepository)
        {
            _contextFactory = contextFactory;
            _roleRepository = roleRepository;
        }

        public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUser(User user)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleId(int roleId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
        }

        public async Task AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            using var context = _contextFactory.CreateDbContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.Status = false;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _roleRepository.GetAllRoles();
        }

        public async Task<Role?> GetRoleById(int roleId)
        {
            return await _roleRepository.GetRoleById(roleId);
        }
    }
}
