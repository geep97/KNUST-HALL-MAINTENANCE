using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IStudentService
{
    Task<Student?> GetByStudentIdAsync(string studentId);

    bool VerifyPassword(Student student, string password);
}