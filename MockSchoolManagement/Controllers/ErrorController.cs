using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class ErrorController:Controller
    {
        private readonly ILogger<ErrorController> logger;

        ///<summary>
        ///注入ASP.NET  Core ILogger服务。
        ///将控制器类型指定为泛型参数。
        ///这有助于我们进行确定哪个类或控制器产生了异常，然后记录它
        ///</summary>
        ///<param name="logger"></param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }


        public IActionResult Error()
        {
            //获取异常详情信息
            var exceptionHandlerPathFeature =
                    HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //LogError() 方法将异常记录作为日志中的错误类别记录
            logger.LogError($"路径 {exceptionHandlerPathFeature.Path} " +
                $"产生了一个错误{exceptionHandlerPathFeature.Error}");
            return View("Error");
        }
    }
}
