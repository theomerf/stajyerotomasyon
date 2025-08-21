using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IServiceManager _manager;

        public ReportsController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index([FromQuery] ReportRequestParameters query)
        {
            var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");
            query.PageSize = cookiePageSize;
            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var reports = await _manager.ReportService.GetAllReportsOfOneUserAsync(query, userId!);
                var totalCount = await _manager.ReportService.GetAllReportsCountOfOneUserAsync(query, userId!);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<ReportDto>()
                {
                    List = reports as List<ReportDto>,
                    Pagination = paginaton,
                };
                return PartialView("_Index", model);
            }
            else
            {
                var reports = await _manager.ReportService.GetAllReportsAsync(query);
                var totalCount = await _manager.ReportService.GetReportsCountAsync(query);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<ReportDto>()
                {
                    List = reports as List<ReportDto>,
                    Pagination = paginaton,
                };
                return PartialView("_Index", model);
            }
        }

        public async Task<IActionResult> View([FromRoute(Name = "id")] int id)
        {
            var model = await _manager.ReportService.GetReportByIdForViewAsync(id);

            return PartialView("_View", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int reportId)
        {
            var result = await _manager.ReportService.ChangeStatusAsync(reportId);

            var report = await _manager.ReportService.GetReportByIdForViewAsync(reportId);

            var html = await this.RenderViewAsync("_View", report, true);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                html = $"<div id='content' hx-swap-oob='true'>{html}</div>",
                type = result.ResultType,
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteReport([FromQuery] int reportId, [FromQuery] ReportRequestParameters query)
        {
            if (User.IsInRole("Admin"))
            {
                var result = await _manager.ReportService.DeleteReportAsync(reportId);

                var reports = await _manager.ReportService.GetAllReportsAsync(query);
                var totalCount = await _manager.ReportService.GetReportsCountAsync(query);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<ReportDto>()
                {
                    List = reports as List<ReportDto>,
                    Pagination = paginaton,
                };

                var htmlList = await this.RenderViewAsync("Small/_ReportsListView", model, true);
                var htmlGrid = await this.RenderViewAsync("Small/_ReportsGridView", model, true);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    html = $"<div id='reportsContent' hx-swap-oob='true'>{htmlList}{htmlGrid}</div>",
                    type = result.ResultType,
                });
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _manager.ReportService.DeleteReportForUserAsync(reportId, userId!);

                var reports = await _manager.ReportService.GetAllReportsOfOneUserAsync(query, userId!);
                var totalCount = await _manager.ReportService.GetAllReportsCountOfOneUserAsync(query, userId!);

                var paginaton = new Pagination()
                {
                    CurrentPage = query.PageNumber,
                    ItemsPerPage = query.PageSize,
                    TotalItems = totalCount
                };

                var model = new ListViewModel<ReportDto>()
                {
                    List = reports as List<ReportDto>,
                    Pagination = paginaton,
                };

                var htmlList = await this.RenderViewAsync("Small/_ReportsListView", model, true);
                var htmlGrid = await this.RenderViewAsync("Small/_ReportsGridView", model, true);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    html = $"<div id='reportsContent' hx-swap-oob='true'>{htmlList}{htmlGrid}</div>",
                    type = result.ResultType,
                });
            }

        }

        public async Task<IActionResult> AddReport([FromQuery] int? workId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workNames = await _manager.WorkService.GetAllWorkNamesOfOneUserAsync(userId!);
            ViewBag.WorkNames = workNames;

            if (workId != null)
            {
                if(workNames.Where(w => w!.WorkId == workId).Select(w => w!.WorkEndDate).FirstOrDefault() > DateTime.Now)
                {
                    var model = new ReportDtoForCreation()
                    {
                        WorkId = workId,
                        WorkName = workNames.Where(w => w!.WorkId == workId).Select(w => w!.WorkName).FirstOrDefault(),
                    };


                    return PartialView("_AddReport", model);
                }
                else
                {
                    return Forbid();
                }

            }
            else
            {
                var model = new ReportDtoForCreation();
                return PartialView("_AddReport", model);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddReport([FromForm] ReportDtoForCreation reportDto, [FromForm] List<IFormFile>? files = null)
        {
            if (reportDto.ImageUrls == null)
            {
                reportDto.ImageUrls = new List<string>();
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    string fileName = $"{Guid.NewGuid().ToString()}.png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/reports", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    reportDto.ImageUrls.Add(fileName);
                }
            }

            reportDto.AccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _manager.ReportService.CreateReportAsync(reportDto);

            return Json(new
            {
                success = result.Success,
                type = result.ResultType,
                loadComponent = result.LoadComponent,
                message = result.Message,
            });
        }

        public async Task<IActionResult> UpdateReport([FromQuery] int reportId)
        {
            var model = await _manager.ReportService.GetReportByIdForUpdateAsync(reportId);
            if(model?.Status == ReportStatus.NotRead.ToString())
            {
                return PartialView("_UpdateReport", model);
            }
            else
            {
                return Forbid();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateReport(ReportDtoForUpdate reportDto, [FromForm] List<IFormFile>? files = null)
        {
            if (reportDto.ImageUrls == null)
            {
                reportDto.ImageUrls = new List<string>();
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    string fileName = $"{Guid.NewGuid()}.png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/reports", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    reportDto.ImageUrls?.Add(fileName);
                }
            }

            if (reportDto.PhotosToDelete != null)
            {
                foreach (var fileName in reportDto.PhotosToDelete)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/reports", fileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                reportDto.ImageUrls = reportDto.ImageUrls?.Where(x => reportDto.PhotosToDelete.Contains(x) == false).ToList();
            }

            reportDto.AccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _manager.ReportService.UpdateReportAsync(reportDto);
            return Json(new
            {
                success = result.Success,
                type = result.ResultType,
                loadComponent = result.LoadComponent,
                message = result.Message,
            });
        }
    }
}
