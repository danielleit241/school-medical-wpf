using Microsoft.AspNetCore.Identity;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly RoleRepository _roleRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(UserRepository repository, RoleRepository roleRepository, PasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher; // DI PasswordHasher
        }

        public async Task<User?> Authenticate(string phoneNumber, string password)
        {
            var user = await _repository.GetUserByPhoneNumber(phoneNumber).ConfigureAwait(false);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            return await _repository.GetUserById(userId).ConfigureAwait(false);
        }

        public async Task UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Cập nhật người dùng trong cơ sở dữ liệu
            await _repository.UpdateUser(user);
        }


        public async Task<List<User>> GetAllUsers()
        {
            var users = await _repository.GetAllUsers().ConfigureAwait(false);
            return users;
        }

        public async Task<List<User>> GetUsersByRoleId(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }

            var users = await _repository.GetUsersByRoleId(roleId).ConfigureAwait(false);
            return users;
        }

        public async Task AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.RoleId == null || user.RoleId == 0)
            {
                throw new ArgumentException("Invalid RoleId", nameof(user.RoleId));
            }

            var role = await _roleRepository.GetRoleById(user.RoleId.Value).ConfigureAwait(false);
            string roleName = role?.RoleName?.ToLower() ?? "user";
            string defaultPassword = $"{roleName}@123";

            user.PasswordHash = _passwordHasher.HashPassword(user, defaultPassword);
            user.UserId = Guid.NewGuid();
            user.Status = true;

            await _repository.AddUser(user).ConfigureAwait(false);
        }

        public async Task DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            await _repository.DeleteUser(userId).ConfigureAwait(false);
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _roleRepository.GetAllRoles().ConfigureAwait(false);
        }

        public async Task<Role?> GetRoleById(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }
            return await _roleRepository.GetRoleById(roleId).ConfigureAwait(false);
        }
    }
}
