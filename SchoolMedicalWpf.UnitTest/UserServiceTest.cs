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

        // Test case for successful login with valid credentials
        [Fact]
        public async Task Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange: Set up the in-memory DB and test data
            var options = GetDbContextOptions();
            var context = new SchoolmedicalWpfContext(options); // Directly create the context

            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(); // Save changes to in-memory DB

            var userRepository = new UserRepository(context);
            var userService = new UserService(userRepository, hash);

            // Act: Attempt to login with correct credentials
            var result = await userService.Authenticate("1234567890", "password123");

            // Assert: Verify login is successful and user details match
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        }

        // Test case for failed login with incorrect credentials
        [Fact]
        public async Task Authenticate_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange: Set up the in-memory DB and test data
            var options = GetDbContextOptions();
            var context = new SchoolmedicalWpfContext(options); // Directly create the context

            var hash = new PasswordHasher<User>();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                PasswordHash = hash.HashPassword(null!, "password123")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(); // Save changes to in-memory DB

            var userRepository = new UserRepository(context);
            var userService = new UserService(userRepository, hash);

            // Act: Attempt to login with incorrect password
            var result = await userService.Authenticate("1234567890", "wrongpassword");

            // Assert: Verify login failed with incorrect password
            Assert.Null(result);
        }
    }
}
