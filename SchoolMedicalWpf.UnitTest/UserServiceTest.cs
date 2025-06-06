using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.UnitTest;

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
        var context = GetDbContext();
        var hash = new PasswordHasher<User>();
        var user = new User
        {
            UserId = Guid.NewGuid(),
            PhoneNumber = "1234567890",
            PasswordHash = hash.HashPassword(null!, "password123")
        };
        context.Users.Add(user);
        context.SaveChanges();

        var repository = new UserRepository(context);
        var service = new UserService(repository);

        var login = await service.Authenticate("1234567890", "password123");
        Assert.NotNull(login);
        Assert.Equal(user.UserId, login.UserId);
        Assert.Equal(user.PhoneNumber, login.PhoneNumber);
        Assert.Equal(user.PasswordHash, login.PasswordHash);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenInputInvalid()
    {
        var context = GetDbContext();
        var hash = new PasswordHasher<User>();
        var user = new User
        {
            UserId = Guid.NewGuid(),
            PhoneNumber = "1234567890",
            PasswordHash = hash.HashPassword(null!, "password123")
        };
        context.Users.Add(user);
        context.SaveChanges();
        var repository = new UserRepository(context);
        var service = new UserService(repository);
        var login = await service.Authenticate("1234567890", "wrongpassword");
        Assert.Null(login);
    }
}
