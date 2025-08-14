using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class WorksController : Controller
    {
        private readonly IServiceManager _manager;

        public WorksController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index([FromQuery]WorkRequestParameters query)
        {
            var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");
            query.PageSize = cookiePageSize;

            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var reports = await _manager.WorkService.GetAllWorksAsync(query);
            var totalCount = await _manager.WorkService.GetWorksCountAsync(query);

            var paginaton = new Pagination()
            {
                CurrentPage = query.PageNumber,
                ItemsPerPage = query.PageSize,
                TotalItems = totalCount
            };

            var model = new ListViewModel<WorkDto>()
            {
                List = reports as List<WorkDto>,
                Pagination = paginaton,
            };
            return PartialView("_Index", model);
        }

        public async Task<IActionResult> AddWork()
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

            return PartialView("_AddWork");
        }

        [HttpPost]
        public async Task<IActionResult> AddWork([FromForm]WorkDtoForCreation workDto, [FromForm]List<IFormFile> files)
        {
            if (workDto.ImageUrls == null)
            {
                workDto.ImageUrls = new List<string>();
            }

            foreach (var file in files) 
            {
                string fileName = $"{Guid.NewGuid().ToString()}.png";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/works", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                workDto.ImageUrls.Add(fileName);
            }

            workDto.TaskMasterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _manager.WorkService.CreateWorkAsync(workDto);

            return Json(new
            {
                success = result.Success,
                type= result.ResultType,
                loadComponent = result.LoadComponent,
                message = result.Message,
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteWork([FromQuery] int workId)
        {
            var result = await _manager.WorkService.DeleteWorkAsync(workId);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                loadComponent = result.LoadComponent,
                type = result.ResultType,
            });
        }

        public async Task<IActionResult> UpdateWork([FromQuery] int workId)
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

            var model = await _manager.WorkService.GetWorkForUpdateByIdAsync(workId);

            ViewBag.DepartmentList = new SelectList(ViewBag.Departments, "DepartmentId", "DepartmentName", model?.DepartmentId);
            ViewBag.SelectedDepartmentId = model?.DepartmentId;
            ViewBag.SelectedSectionId = model?.SectionId;

            return PartialView("_UpdateWork", model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateWork([FromForm] WorkDtoForUpdate workDto)
        {
            var result = await _manager.WorkService.UpdateWorkAsync(workDto);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                loadComponent = result.LoadComponent,
                type = result.ResultType,
            });
        }
    }
}
