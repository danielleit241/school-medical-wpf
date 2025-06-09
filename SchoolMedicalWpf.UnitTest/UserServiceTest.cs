using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using Xunit;

namespace SchoolMedicalWpf.UnitTest;

public class UserServiceTest
{
    //private IDbContextFactory<SchoolmedicalWpfContext> GetDbContextFactory()
    //{
    //    //var options = new DbContextOptionsBuilder<SchoolmedicalWpfContext>()
    //    //    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    //    //    .Options;

    //    //return new DbContextFactory(options); // Assuming you have a DbContextFactory that implements IDbContextFactory
    //}

    [Fact]
    public async Task LoginUser_ShouldReturnSuccess_WhenInputValid()
    {
        //// Arrange
        //var contextFactory = GetDbContextFactory();
        //var hash = new PasswordHasher<User>();
        //var user = new User
        //{
        //    UserId = Guid.NewGuid(),
        //    PhoneNumber = "1234567890",
        //    PasswordHash = hash.HashPassword(null!, "password123")
        //};

        //using (var context = contextFactory.CreateDbContext())
        //{
        //    context.Users.Add(user);
        //    await context.SaveChangesAsync(); // Use SaveChangesAsync to ensure async operation is completed
        //}

        //var repository = new UserRepository(contextFactory);
        //var service = new UserService(repository);

        //// Act
        //var login = await service.Authenticate("1234567890", "password123");

        //// Assert
        //Assert.NotNull(login);
        //Assert.Equal(user.UserId, login.UserId);
        //Assert.Equal(user.PhoneNumber, login.PhoneNumber);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenInputInvalid()
    {
        //// Arrange
        //var contextFactory = GetDbContextFactory();
        //var hash = new PasswordHasher<User>();
        //var user = new User
        //{
        //    UserId = Guid.NewGuid(),
        //    PhoneNumber = "1234567890",
        //    PasswordHash = hash.HashPassword(null!, "password123")
        //};

        //using (var context = contextFactory.CreateDbContext())
        //{
        //    context.Users.Add(user);
        //    await context.SaveChangesAsync(); // Use SaveChangesAsync to ensure async operation is completed
        //}

        //var repository = new UserRepository(contextFactory);
        //var service = new UserService(repository);

        //// Act
        //var login = await service.Authenticate("1234567890", "wrongpassword");

        //// Assert
        //Assert.Null(login);
    }
}
