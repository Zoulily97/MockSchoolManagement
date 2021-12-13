using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentsCourseId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }

    }
}
