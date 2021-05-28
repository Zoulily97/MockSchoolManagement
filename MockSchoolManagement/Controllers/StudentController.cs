using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StudentController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            IEnumerable<Student> model = _studentRepository.GetAllStudents();
            return View(model);
        }
        public IActionResult Details(int id)
        {
            DetailViewModel detailViewModel = new DetailViewModel()
            {
                Student = _studentRepository.GetStudent(id),
                PageTitle = "学生详情"

            };
            return View(detailViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        //必须将图像上传到wwwroot中的images文件夹
                        //而要获取wwwroot文件夹的路径，我们需要注入 ASP.NET  Core提供的HostingEnvironment服务
                        //通过HostingEnvironment服务去获取wwwroot文件夹的路径
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images" );
                        //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        //使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images文件夹
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    // 将文件名保存在student对象的PhotoPath属性中。
                    //它将保存到数据库 Students的 表中
                    PhotoPath = uniqueFileName
                };
                 _studentRepository.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }

            return View();
        }
    }
}
