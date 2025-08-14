using Entities.Dtos;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Stajyeryotom.Models;
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
}
