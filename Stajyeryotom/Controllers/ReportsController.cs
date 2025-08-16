using Entities.Dtos;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System.Security.Claims;
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

        public async Task<IActionResult> Index([FromQuery]ReportRequestParameters query)
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

        public async Task<IActionResult> View([FromRoute(Name = "id")]int id)
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
        public async Task<IActionResult> DeleteReport([FromQuery] int reportId)
        {
            if (User.IsInRole("Admin"))
            {
                var result = await _manager.ReportService.DeleteReportAsync(reportId);

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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _manager.ReportService.DeleteReportForUserAsync(reportId, userId!);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    loadComponent = result.LoadComponent,
                    type = result.ResultType,
                });
            }

        }

        public async Task<IActionResult> AddReport([FromQuery] int? workId = null)
        {
            if (workId != null) 
            {
                var report = await _manager.WorkService.GetWorkByIdAsync(workId.Value);
            }

            return PartialView("_AddReport");
        }

        public async Task<IActionResult> UpdateReport([FromRoute] int reportId)
        {
            return PartialView("_UpdateReport");
        }
    }
}
