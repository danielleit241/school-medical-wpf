using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;

public class StudentRepository
{
    private readonly DbContextOptions<SchoolmedicalWpfContext> _options;

    public StudentRepository(DbContextOptions<SchoolmedicalWpfContext> options)
    {
        _options = options;
    }

    public async Task<List<Student>> GetAllStudents()
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Students.ToListAsync();
    }

    public async Task<Student?> GetStudentById(Guid studentId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task AddStudent(Student student)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        context.Students.Add(student);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStudent(Student student)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        context.Students.Update(student);
        await context.SaveChangesAsync();
    }

    public async Task DeleteStudent(Guid studentId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        var student = await context.Students.FindAsync(studentId);
        if (student != null)
        {
            context.Students.Remove(student);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Student>> GetStudentsByUserId(Guid userId)
    {
        using var context = new SchoolmedicalWpfContext(_options);
        return await context.Students
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
}