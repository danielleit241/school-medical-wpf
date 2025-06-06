using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class UserRepository
    {
        private readonly SchoolmedicalWpfContext _context;

        public UserRepository(SchoolmedicalWpfContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}