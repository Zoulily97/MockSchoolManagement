using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
   // [Authorize(Roles = "Student")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;          
        private readonly ILogger<AdminController> _logger;
        public AdminController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        
        {
            this._roleManager = roleManager;
           this. _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region 角色管理
        [HttpHead]

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
       
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);

        }
        #endregion

        #region 新增角色
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                //角色保存在Role表中
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region 删除角色
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"角色Id={id}的信息不存在，请重试";
                return View("NotFound");

            }
            else
            { //将代码包装在trycatch中
                try
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                ///如果触发的异常是DbUpdateException，我们知道我们无法删除角色，
                ///因为该角色中已存在用户信息
                catch(DbUpdateException ex)
                {
                    //将异常记录到文件中。我们之前已经学习了使用Nlog配置我们的日志信息
                    _logger.LogError($"发生异常：{ex}");
                    //我们使用ViewBag.ErrorTitle和 ViewBag.ErrorMessage来传递错误标题和详情信息到我们的Error视图。
                    //Error视图会将这些数据显示给用户                    
                    ViewBag.ErrorTitle = $"角色：{role.Name} 正在被使用中...";
                    ViewBag.ErrorMessage = $" 无法删除{role.Name}角色，因为此角色中已经存在用户。如果您想删除此角色，需要先从该角色中删除用户，然后尝试删除该角色本身。";
                    return View("Error");

                }
            }
        }
        #endregion

        #region 编辑角色
        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]  //策略授权
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={id}的信息不存在，请重试";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={model.Id}的信息不存在，请重试";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }
        #endregion

        #region 编辑角色中的用户
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            //通过roleId查询角色实体信息
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                //判断用户是否存在角色中
                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);
                if (isInRole)
                {
                    //存在，true
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return View(model);
        }
      
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            //检查当前角色是否存在
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试。";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;
                //检查当前的userid，是否被选中，如果被选中了则添加到角色中。

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }//对于没有选中的则从userroles表中移除。
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                { //对于其他情况不做处理，继续新的循环。
                    continue;
                }

                if (result.Succeeded)
                {   //判断当前用户是否为最后一个用户，如果是则跳转回EditRole视图，如果不是则进入下一个循环
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        #endregion
        #region 删除角色1
        [HttpPost]
        //public async Task<IActionResult> DeleteRole(string id)
        //{
        //    var role = await _roleManager.FindByIdAsync(id);
        //    if (role == null)
        //    {
        //        ViewBag.ErrorMessage = $"角色={id}的信息不存在，请重试。";
        //        return View("NotFound");
        //    }
        //    else
        //    {
        //        var result = await _roleManager.DeleteAsync(role);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("ListRoles");
        //        }
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //        return View("ListRoles");
        //    }

        //}
        #endregion
        

        #region 用户管理
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);

        }
        #endregion
        #region 编辑用户
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"用户Id={id}的信息不存在，请重试";
                return View("NotFound");
            }
            //用户声明
            var userClaims = await _userManager.GetClaimsAsync(user);
            //用户角色
            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
               // Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles,
                Claims=userClaims

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"用户Id={model.Id}的信息不存在，请重试";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        [HttpGet]
      public async Task<IActionResult>  ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"用户Id={userId}的信息不存在，请重试";
                return View("NotFound");
            }
            var model = new List<RolesInUserViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var roleInUserViewModel = new RolesInUserViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                //判断当前账户是否拥有该角色信息
                if (await _userManager.IsInRoleAsync(user,role.Name))
                {
                    roleInUserViewModel.IsSelected = true;
                }
                else
                {
                    roleInUserViewModel.IsSelected = false;
                }
                model.Add(roleInUserViewModel);
            }
            //添加已有角色到视图模型
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<RolesInUserViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"用户Id={userId}的信息不存在，请重试";
                return View("NotFound");
            }
            var roles = await _userManager.GetRolesAsync(user);
            //MARS
            //var roles = await _roleManager.Roles.ToListAsync();
            //foreach (var role in roles)
            //{

            //}

            //移除当前用户中的所有角色信息
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除用户中的现有角色");
                return View(model);
            }
            //查询出模型列表中被选中的rolename添加到用户中。
            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的角色");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = userId });


        }
        #endregion

        #region 用户声明
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"用户Id={userId}的信息不存在，请重试";
                return View("NotFound"); 
            }
                // UserManager服务中的GetClaimsAsync方法获取用户当前的所有声明
                var existingUserClaims = await _userManager.GetClaimsAsync(user);
                var model = new UserClaimsViewModel
                {
                    UserId = userId
                };
            // 循环遍历应用程序中的每个声明
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // 如果用户选中了声明属性，设置IsSelected属性为true， 
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{model.UserId}的用户";
                return View("NotFound");
            }
            // //获取所有用户现有的声明并删除它们
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除当前用户的声明");
                return View(model);
            }
            //添加页面上选中的声明信息
            //result = await _userManager.AddClaimsAsync(user,
            //    model.Cliams.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));
            result = await _userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType,c.IsSelected ? "true": "false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的声明");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = model.UserId });




        }

        #endregion
        #region 删除用户
        //[HttpGet]
        //public IActionResult DeleteUser()
        //{
        //    var users =  _userManager.Users.ToList();
        //    return View(users);
        //}
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"用户={id}的信息不存在，请重试。";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }

        }
        #endregion


    }
}
