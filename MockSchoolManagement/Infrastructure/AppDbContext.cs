using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
            //指定实体在数据库中生成的名字
            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<Student >().ToTable("Student");

            //获取系统领域模型外键列表
            var foreginleys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var foreignKey in foreginleys)
            {
                //将删除行为配置为restrict ，无操作
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
