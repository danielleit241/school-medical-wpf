using Microsoft.AspNetCore.Identity;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Bll.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        // Inject UserRepository and PasswordHasher into UserService
        public UserService(UserRepository userRepository, PasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        // Authenticate user with phone number and password
        public async Task<User?> Authenticate(string phoneNumber, string password)
        {
            var user = await _userRepository.GetUserByPhoneNumber(phoneNumber).ConfigureAwait(false);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;
        }

        // Get user by ID
        public async Task<User?> GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            return await _userRepository.GetUserById(userId).ConfigureAwait(false);
        }

        // Update user information
        public async Task<bool> UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _userRepository.UpdateUser(user).ConfigureAwait(false);
            return true;
        }

        // Get all users
        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers().ConfigureAwait(false);
        }

        // Get users by role ID
        public async Task<List<User>> GetUsersByRoleId(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }
            return await _userRepository.GetUsersByRoleId(roleId).ConfigureAwait(false);
        }

        // Add a new user
        public async Task AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            string defaultPassword = "123@123@123";  // Default password logic
            user.PasswordHash = _passwordHasher.HashPassword(user, defaultPassword);
            user.UserId = Guid.NewGuid();
            user.Status = true;

            await _userRepository.AddUser(user).ConfigureAwait(false);
        }

        // Delete user by ID
        public async Task DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            await _userRepository.DeleteUser(userId).ConfigureAwait(false);
        }

        public async Task<bool>? ChangePassword(string phone, string oldPass, string newPass)
        {
            var user = await _userRepository.GetUserByPhoneNumber(phone).ConfigureAwait(false);
            if (user == null)
            {
                throw new ArgumentException("User not found with the provided phone number.", nameof(phone));
            }
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPass);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, newPass);
                await _userRepository.UpdateUser(user).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
