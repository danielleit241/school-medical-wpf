using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using Xunit;

namespace SchoolMedicalWpf.UnitTest
{
    public class UserServiceTest
    {
        private DbContextOptions<SchoolmedicalWpfContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<SchoolmedicalWpfContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        // Fix for CS7036: Adjust the instantiation of DbContextFactory to provide the required parameters.

        private IDbContextFactory<SchoolmedicalWpfContext> GetDbContextFactory()
        {
            var options = GetDbContextOptions();
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            return new DbContextFactory<SchoolmedicalWpfContext>(
                serviceProvider,
                options,
                new DbContextFactorySource<SchoolmedicalWpfContext>()
            );
        }

        [Fact]
        public async Task LoginUser_ShouldReturnSuccess_WhenInputValid()
        {
            var dbContextFactory = GetDbContextFactory(); // Get DbContextFactory instance
            var context = dbContextFactory.CreateDbContext(); // Create context using factory

            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(); // Save changes to in-memory DB

            var repository = new UserRepository(dbContextFactory, new RoleRepository(dbContextFactory));
            var service = new UserService(repository, new RoleRepository(dbContextFactory), hash);

            var login = await service.Authenticate("1234567890", "password123");

            Assert.NotNull(login);
            Assert.Equal(user.UserId, login.UserId);
            Assert.Equal(user.PhoneNumber, login.PhoneNumber);
            Assert.Equal(user.PasswordHash, login.PasswordHash);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenInputInvalid()
        {
            var dbContextFactory = GetDbContextFactory(); // Get DbContextFactory instance
            var context = dbContextFactory.CreateDbContext(); // Create context using factory

            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(); // Save changes to in-memory DB

            var repository = new UserRepository(dbContextFactory, new RoleRepository(dbContextFactory));
            var service = new UserService(repository, new RoleRepository(dbContextFactory), hash);

            var login = await service.Authenticate("1234567890", "wrongpassword");
            Assert.Null(login);
        }
    }
}
