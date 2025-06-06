using Microsoft.AspNetCore.Identity;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;


namespace SchoolMedicalWpf.Bll.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> Authenticate(string phoneNumber, string password)
        {
            var user = await _repository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return user;
        }
    }
}