using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.UnitTest
{
    public class UserServiceTest
    {
        // Helper function to get an in-memory DbContext options for SchoolmedicalWpfContext
        private DbContextOptions<SchoolmedicalWpfContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<SchoolmedicalWpfContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var options = GetDbContextOptions();
            var hash = new PasswordHasher<User>();

            // Seed data
            using (var context = new SchoolmedicalWpfContext(options))
            {
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    PhoneNumber = "1234567890",
                    PasswordHash = hash.HashPassword(null!, "password123")
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Create repository & service
            var userRepository = new UserRepository(options);
            var userService = new UserService(userRepository, hash);

            // Act
            var result = await userService.Authenticate("1234567890", "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1234567890", result.PhoneNumber);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var options = GetDbContextOptions();
            var hash = new PasswordHasher<User>();

            using (var context = new SchoolmedicalWpfContext(options))
            {
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    PhoneNumber = "1234567890",
                    PasswordHash = hash.HashPassword(null!, "password123")
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var userRepository = new UserRepository(options);
            var userService = new UserService(userRepository, hash);

            // Act
            var result = await userService.Authenticate("1234567890", "wrongpassword");

            // Assert
            Assert.Null(result);
        }
    }
}
