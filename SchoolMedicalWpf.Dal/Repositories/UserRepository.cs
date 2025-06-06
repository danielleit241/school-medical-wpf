using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class UserRepository
    {
        private SchoolmedicalWpfContext _context;

        public async Task<User?> Login(string phoneNumber, string password)
        {
            _context = new();

            var user = await _context.Users
                .Where(u => u.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
    }
}
