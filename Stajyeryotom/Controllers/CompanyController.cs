using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;

namespace Stajyeryotom.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly IServiceManager _manager;

        public CompanyController(IServiceManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index()
        {
            return PartialView("_Index");
        }

        public IActionResult AddDepartment()
        {
            var model = new DepartmentDto();
            return PartialView("_AddDepartment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment([FromForm] DepartmentDto departmentDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _manager.DepartmentService.CreateDepartmentAsync(departmentDto);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    loadComponent = result.LoadComponent,
                    type = result.ResultType,
                });
            }
            else
            {
                var html = await this.RenderViewAsync("_AddDepartment", departmentDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Departman oluştururken bir hata oluştu.", type = "danger" });
            }
        }

        public async Task<IActionResult> AddSection()
        {
            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var model = new SectionCreateModel();
            return PartialView("_AddSection", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSection(SectionCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _manager.SectionService.CreateSectionsAsync(model.DepartmentId!,model.SectionNames);
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    loadComponent = result.LoadComponent,
                    type = result.ResultType,
                });
            }
            else
            {
                var html = await this.RenderViewAsync("_AddSection", model, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Bölüm oluştururken bir hata oluştu.", type = "danger" });
            }
        }

        public async Task<IActionResult> AddUser()
        {
            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var sections = await _manager.SectionService.GetAllSectionsAsync();
            var departmentSections = sections
                .GroupBy(s => s.DepartmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new { id = s.SectionId, name = s.SectionName }).ToList()
                );
            ViewBag.DepartmentSections = departmentSections;
            ViewBag.DepartmentSectionsJson = System.Text.Json.JsonSerializer.Serialize(departmentSections);
            var userNo = await _manager.AuthService.GenerateNewUserNumberAsync();
            var model = new AccountDtoForCreation()
            {
                UserName = userNo,
                Roles = new HashSet<string?>(_manager
                .AuthService
                .Roles
                .Select(r => r.Name)
                .ToList())
            };
            return PartialView("_AddUser", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm] AccountDtoForCreation accountDto)
        {
            if (!ModelState.IsValid)
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId, accountDto.SectionId);
                var html = await this.RenderViewAsync("_AddUser", accountDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Kullanıcı oluşturulurken form hatası oluştu.", type = "warning" });
            }

            var result = await _manager.AuthService.CreateUserAsync(accountDto);

            if (result.Succeeded)
            {
                return Json(new
                {
                    success = true,
                    message = "Kullanıcı başarıyla oluşturuldu.",
                    loadComponent = "Interns",
                    type = "success"
                });
            }
            else
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId, accountDto.SectionId);
                var html = await this.RenderViewAsync("_AddUser", accountDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Lütfen kurallara uygun şifre girin.", type = "danger" });
            }

        }

        private async Task PrepareDropdownsAsync(int selectedDepartmentId, int selectedSectionId)
        {
            var departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;

            var sections = await _manager.SectionService.GetAllSectionsAsync();
            var departmentSections = sections
                .GroupBy(s => s.DepartmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new { id = s.SectionId, name = s.SectionName }).ToList()
                );

            ViewBag.DepartmentSections = departmentSections;
            ViewBag.DepartmentSectionsJson = System.Text.Json.JsonSerializer.Serialize(departmentSections);

            ViewBag.DepartmentList = new SelectList(departments, "DepartmentId", "DepartmentName", selectedDepartmentId);
            ViewBag.SelectedDepartmentId = selectedDepartmentId;
            ViewBag.SelectedSectionId = selectedSectionId;
        }
    }
}
