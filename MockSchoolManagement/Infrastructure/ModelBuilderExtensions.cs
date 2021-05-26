using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 2,
                    Name = "zouli",
                    Major = Models.EnumTypes.MajorEnum.Csharp,
                    Email = "1178.com"

                }
                );
            modelBuilder.Entity<Student>().HasData(
               new Student
               {
                   Id = 3,
                   Name = "lisi",
                   Major = Models.EnumTypes.MajorEnum.Java,
                   Email = "1178lisi.com"

               }
               );
        }

    }
}
