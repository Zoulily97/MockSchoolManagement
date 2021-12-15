using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class WelcomeController : Controller
    {
        private readonly IRepository<Student, int> _studentrepository;
        public WelcomeController(IRepository<Student,int> studentrepository)
        {
            _studentrepository = studentrepository;

        }

        public async Task<string > Index()
        {
            var student =await  _studentrepository.GetAll().FirstOrDefaultAsync();
            var oop = await _studentrepository.SingleAsync(a => a.Id == 6);
            return $"{student.Name}++{oop.Name}";

        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
