using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FPFI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FPFI.Models.ViewModels;

namespace BiblioMit.Controllers
{
    [Authorize(Policy = "Users")]
    public class ApplicationRolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        
        public ApplicationRolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ApplicationRoleListViewModel> model = new List<ApplicationRoleListViewModel>();
            model = _roleManager.Roles.Select(r => new ApplicationRoleListViewModel
            {
                ID = r.Id,
                RoleName = r.Name,
                Description = r.Description,
                NumberOfUsers = r.Users.Count
            }).ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles ="Editor,Administrator")]
        public async Task<IActionResult> AddEditApplicationRole(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    model.ID = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return PartialView("_AddEditApplicationRole", model);
        }
        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public async Task<IActionResult> AddEditApplicationRole(string id, ApplicationRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExist = !String.IsNullOrEmpty(id);
                ApplicationRole applicationRole = isExist ? await _roleManager.FindByIdAsync(id) :
               new ApplicationRole
               {
                   CreatedDate = DateTime.UtcNow
               };
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                IdentityResult roleRuslt = isExist ? await _roleManager.UpdateAsync(applicationRole)
                                                    : await _roleManager.CreateAsync(applicationRole);
                if (roleRuslt.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteApplicationRole(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    name = applicationRole.Name;
                }
            }
            return PartialView("_DeleteApplicationRole", name);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteApplicationRole(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = _roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
    }
}