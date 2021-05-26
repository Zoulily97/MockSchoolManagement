using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    public class StudentRepository : IStudentRepository
    {
        private List<Student> _studentList;
        public StudentRepository()
        {
            _studentList = new List<Student>()
            {
                new Student()
                {
                    Id=1,Name="zhangSan",Major=MajorEnum.Csharp,Email="zouli.qq.com"
                },
                new Student()
                {
                    Id=2,Name="San",Major=MajorEnum.Java,Email="zouli.qq.com"
                },
            };

        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _studentList;
        }

        public Student GetStudent(int id)
        {
            return _studentList.FirstOrDefault(a => a.Id == id);
        }

        public Student Insert(Student student)
        {
            student.Id = _studentList.Max(s => s.Id) + 1;
            _studentList.Add(student);
            return student;
        }

        public Student Delete(int id)
        {
            Student student = _studentList.FirstOrDefault(student => student.Id == id);
            if (student!=null)
            {
                _studentList.Remove(student);
            }
            return student;
        }

        public Student Update(Student updatestudent)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == updatestudent.Id);
            if (student!=null)
            {
                student.Name = updatestudent.Name;
                student.Email = updatestudent.Email;
                student.Major = updatestudent.Major;
            }
            return student;
        }
    }
}
