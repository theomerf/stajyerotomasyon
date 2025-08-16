using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System;
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

        public async Task<IActionResult> Index([FromQuery] WorkRequestParameters query)
        {
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var works = await _manager.WorkService.GetAllWorksOfOneUserAsync(userId!);
                var totalCount = await _manager.WorkService.GetAllWorksCountOfOneUser(userId!);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<WorkDto>()
                {
                    List = works as List<WorkDto>,
                    Pagination = paginaton,
                };
                return PartialView("_Index", model);

            }
            else
            {
                var works = await _manager.WorkService.GetAllWorksAsync(query);
                var totalCount = await _manager.WorkService.GetWorksCountAsync(query);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<WorkDto>()
                {
                    List = works as List<WorkDto>,
                    Pagination = paginaton,
                };
                return PartialView("_Index", model);
            }

        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddWork([FromForm] WorkDtoForCreation workDto, [FromForm] List<IFormFile>? files = null)
        {
            if (workDto.ImageUrls == null)
            {
                workDto.ImageUrls = new List<string>();
            }

            if (files != null)
            {
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
            }

            workDto.TaskMasterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _manager.WorkService.CreateWorkAsync(workDto);

            return Json(new
            {
                success = result.Success,
                type = result.ResultType,
                loadComponent = result.LoadComponent,
                message = result.Message,
            });
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateWork([FromForm] WorkDtoForUpdate workDto, [FromForm] List<IFormFile>? files = null)
        {
            if (workDto.ImageUrls == null)
            {
                workDto.ImageUrls = new List<string>();
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    string fileName = $"{Guid.NewGuid()}.png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/works", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    workDto.ImageUrls?.Add(fileName);
                }
            }

            if (workDto.PhotosToDelete != null)
            {
                foreach (var fileName in workDto.PhotosToDelete)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/works", fileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                workDto.ImageUrls = workDto.ImageUrls?.Where(x => workDto.PhotosToDelete.Contains(x) == false).ToList();
            }

            var result = await _manager.WorkService.UpdateWorkAsync(workDto);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                loadComponent = result.LoadComponent,
                type = result.ResultType,
            });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int workId)
        {
            var result = await _manager.WorkService.ChangeStatusAsync(workId);

            var work = await _manager.WorkService.GetWorkByIdForViewAsync(workId);

            var html = await this.RenderViewAsync("_View", work, true);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                html = $"<div id='content' hx-swap-oob='true'>{html}</div>",
                type = result.ResultType,
            });
        }

        public async Task<IActionResult> View([FromRoute(Name = "id")] int id)
        {
            if (User.IsInRole("Admin"))
            {
                var model = await _manager.WorkService.GetWorkByIdForViewAsync(id);

                return PartialView("_View", model);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _manager.WorkService.GetWorkByIdForViewOfOneUserAsync(id, userId!);

                return PartialView("_View", model);
            }

        }
    }
}
