using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using Moq;

namespace SchoolMedicalWpf.UnitTest
{
    public class UserServiceTest
    {
        private SchoolmedicalWpfContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SchoolmedicalWpfContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SchoolmedicalWpfContext(options);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnSuccess_WhenInputValid()
        {
            var context = GetDbContext();  // New DbContext instance for each test
            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var roleRepository = new RoleRepository(context);  // Added RoleRepository instance
            var repository = new UserRepository(context, roleRepository);  // Pass RoleRepository to UserRepository
            var service = new UserService(repository, roleRepository, hash);

            var login = await service.Authenticate("1234567890", "password123");

            Assert.NotNull(login);
            Assert.Equal(user.UserId, login.UserId);
            Assert.Equal(user.PhoneNumber, login.PhoneNumber);
            Assert.Equal(user.PasswordHash, login.PasswordHash);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenInputInvalid()
        {
            var context = GetDbContext();  // New DbContext instance for each test
            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var roleRepository = new RoleRepository(context);  // Added RoleRepository instance
            var repository = new UserRepository(context, roleRepository);  // Pass RoleRepository to UserRepository
            var service = new UserService(repository, roleRepository, hash);

            var login = await service.Authenticate("1234567890", "wrongpassword");

            Assert.Null(login);
        }
    }

}
