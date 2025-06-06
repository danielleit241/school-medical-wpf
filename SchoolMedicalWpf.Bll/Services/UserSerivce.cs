using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class UserService
    {
        public readonly UserRepository repository = new();

        public async Task<User?> Authenticate(string phoneNumber, string password)
        {
            return await repository.Login(phoneNumber, password);
        }
    }
}
