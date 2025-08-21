using Entities.Dtos;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Stajyeryotom.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InternsController : Controller
    {
        private readonly IServiceManager _manager;

        public InternsController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index([FromQuery]AccountRequestParameters query)
        {
            var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");
            query.PageSize = cookiePageSize;

            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var interns = await _manager.AuthService.GetAllInternsAsync(query);
            var totalCount = await _manager.AuthService.GetAllInternsCountAsync(query);

            var paginaton = new Pagination()
            {
                CurrentPage = query.PageNumber,
                ItemsPerPage = query.PageSize,
                TotalItems = totalCount
            };

            var model = new ListViewModel<AccountDto>()
            {
                List = interns as List<AccountDto>,
                Pagination = paginaton,
            };

            return PartialView("_Index", model);
        }

        public async Task<IActionResult> AddIntern(int? applicationId = null)
        {
            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var sections = await _manager.SectionService.GetAllSectionsAsync();

            TempData["ApplicationId"] = applicationId;

            var departmentSections = sections
                .GroupBy(s => s.DepartmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new { id = s.SectionId, name = s.SectionName }).ToList()
                );

            ViewBag.DepartmentSections = departmentSections;

            ViewBag.DepartmentSectionsJson = System.Text.Json.JsonSerializer.Serialize(departmentSections);
            var userNo = await _manager.AuthService.GenerateNewUserNumberAsync();

            if (applicationId != null)
            {
                TempData["ApplicationId"] = applicationId.Value;
                var model = await _manager.ApplicationService.GetApplicationForExportAsync(applicationId.Value, userNo);
                TempData["SectionId"] = model.SectionId;
                return PartialView("_AddIntern", model);
            }
            else
            {
                var model = new AccountDtoForCreation()
                {
                    UserName = userNo
                };
                return PartialView("_AddIntern", model);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddIntern([FromForm] AccountDtoForCreation accountDto, int? applicationId = null)
        {
            if (!ModelState.IsValid)
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId!.Value, accountDto.SectionId!.Value);
                var html = await this.RenderViewAsync("_AddIntern", accountDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Stajyer oluşturulurken form hatası oluştu.", type = "warning" });
            }
            var result = await _manager.AuthService.CreateUserAsync(accountDto);

            if (result.Succeeded)
            {
                if(applicationId != null)
                {
                    await _manager.ApplicationService.ExportToInternsAsync(applicationId.Value);
                }

                return Json(new
                {
                    success = true,
                    message = "Stajyer başarıyla oluşturuldu.",
                    loadComponent = "Interns",
                    type = "success"
                });

            }
            else
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId!.Value, accountDto.SectionId!.Value);
                var html = await this.RenderViewAsync("_AddIntern", accountDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Lütfen kurallara uygun şifre girin.", type = "danger" });
            }

        }

        public async Task<IActionResult> UpdateIntern([FromQuery] string userName)
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

            var model = await _manager.AuthService.GetOneUserForUpdateAsync(userName);

            ViewBag.DepartmentList = new SelectList(departments, "DepartmentId", "DepartmentName", model.DepartmentId);
            ViewBag.SelectedDepartmentId = model.DepartmentId;
            ViewBag.SelectedSectionId = model.SectionId;

            return PartialView("_UpdateIntern", model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateIntern([FromForm] AccountDtoForUpdate accountDto)
        {
            if (!ModelState.IsValid)
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId!.Value, accountDto.SectionId!.Value);
                var html = await this.RenderViewAsync("_UpdateIntern", accountDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Stajyer güncellenirken form hatası oluştu.", type = "warning" });
            }

            var result = await _manager.AuthService.UpdateAsync(accountDto);

            if (result.Succeeded)
            {
                return Json(new
                {
                    success = true,
                    message = "Stajyer başarıyla güncellendi.",
                    loadComponent = "Interns",
                    type = "success"
                });
            }
            else
            {
                await PrepareDropdownsAsync(accountDto.DepartmentId!.Value, accountDto.SectionId!.Value);
                var html = await this.RenderViewAsync("_UpdateIntern", accountDto, true);
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteIntern([FromQuery] string userName, [FromQuery] AccountRequestParameters query)
        {
            var result = await _manager.AuthService.DeleteOneUserAsync(userName);
            if (result.Succeeded)
            {
                var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");
                query.PageSize = cookiePageSize;

                var interns = await _manager.AuthService.GetAllInternsAsync(query);
                var totalCount = await _manager.AuthService.GetAllInternsCountAsync(query);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<AccountDto>()
                {
                    List = interns as List<AccountDto>,
                    Pagination = paginaton,
                };

                var htmlList = await this.RenderViewAsync("Small/_InternsListView", model, true);
                var htmlGrid = await this.RenderViewAsync("Small/_InternsGridView", model, true);

                return Json(new
                {
                    success = "Success",
                    message = "Stajyer başarıyla silindi.",
                    html = $"<div id='internsContent' hx-swap-oob='true'>{htmlList}{htmlGrid}</div>",
                    type = "Success"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Stajyer silinirken hata oluştu.",
                    loadComponent = "Interns",
                    type = "error"
                });
            }
        }

        public async Task<IActionResult> SearchInterns([FromQuery] string userName)
        {
            var users = await _manager.AuthService.SearchInterns(userName);
            return Json(users);
        }

        public async Task<IActionResult> GetInternsByIds([FromBody]List<string> internsIds)
        {
            var interns = await _manager.AuthService.GetInternsByIds(internsIds);
            return Json(interns);
        }

        public async Task<IActionResult> ChangeStatus([FromQuery]string userName, [FromQuery]AccountRequestParameters query)
        {
            var result = await _manager.AuthService.ChangeStatus(userName);

            var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");

            query.PageSize = cookiePageSize;

            var interns = await _manager.AuthService.GetAllInternsAsync(query);
            var totalCount = await _manager.AuthService.GetAllInternsCountAsync(query);

            var paginaton = new Pagination()
            {
                CurrentPage = query.PageNumber,
                ItemsPerPage = query.PageSize,
                TotalItems = totalCount
            };

            var model = new ListViewModel<AccountDto>()
            {
                List = interns as List<AccountDto>,
                Pagination = paginaton,
            };

            var htmlList = await this.RenderViewAsync("Small/_InternsListView", model, true);
            var htmlGrid = await this.RenderViewAsync("Small/_InternsGridView", model, true);

            return Json(new
            {
                success = "Success",
                message = "Stajyer durumu başarıyla güncellendi.",
                html = $"<div id='internsContent' hx-swap-oob='true'>{htmlList}{htmlGrid}</div>",
                type = "Success"
            });
        }
    }
}
