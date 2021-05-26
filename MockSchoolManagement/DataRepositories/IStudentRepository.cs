using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    public interface IStudentRepository
    {
        Student GetStudent(int id);
        Student Insert(Student student);
        IEnumerable<Student> GetAllStudents();
        Student Delete(int id);
        Student Update(Student updatestudent);
    }
}
