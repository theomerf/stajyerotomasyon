using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Stajyeryotom.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly IServiceManager _manager;

        public ApplicationsController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index([FromQuery] ApplicationRequestParameters query)
        {
            var cookiePageSize = int.Parse(Request.Cookies["PageSize"] ?? "6");
            query.PageSize = cookiePageSize;

            ViewBag.Departments = await _manager.DepartmentService.GetAllDepartmentsAsync();
            var applications = await _manager.ApplicationService.GetAllApplicationsAsync(query);
            var totalCount = await _manager.ApplicationService.GetApplicationsCountAsync(query);

            var paginaton = new Pagination()
            {
                CurrentPage = query.PageNumber,
                ItemsPerPage = query.PageSize,
                TotalItems = totalCount
            };

            var model = new ListViewModel<ApplicationDto>()
            {
                List = applications as List<ApplicationDto>,
                Pagination = paginaton,
            };
            return PartialView("_Index", model);
        }

        public async Task<IActionResult> View([FromRoute(Name="id")] int applicationId)
        {
            var application = await _manager.ApplicationService.ChangeApplicationSeenAsync(applicationId);

            return PartialView("_View", application);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteApplication([FromQuery] int applicationId)
        {
            var result = await _manager.ApplicationService.DeleteApplicationAsync(applicationId);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                loadComponent = result.LoadComponent,
                type = result.ResultType,
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromQuery] int applicationId, string status)
        {
            var result = await _manager.ApplicationService.ChangeApplicationStatusAsync(applicationId,status);
            var application = await _manager.ApplicationService.GetApplicationByIdAsync(applicationId);

            var html = await this.RenderViewAsync("_View", application, true);
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
        public IActionResult ExportToInterns([FromQuery] int applicationId)
        {
            return RedirectToAction("AddIntern", "Interns", new { applicationId = applicationId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddNote([FromQuery] int applicationId, string note)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _manager.ApplicationService.AddNoteToApplicationAsync(applicationId, note, userId!);
            var notes = await _manager.ApplicationService.GetAllNotesForOneApplicationAsync(applicationId);

            var html = await this.RenderViewAsync("Small/_Notes",notes,true);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                html = $"<div id='notes-section' hx-swap-oob='true'>{html}</div>",
                type = result.ResultType,
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RemoveNote([FromQuery] int applicationId, int noteId)
        {
            var result = await _manager.ApplicationService.RemoveNoteFromApplicationAsync(applicationId, noteId);
            var notes = await _manager.ApplicationService.GetAllNotesForOneApplicationAsync(applicationId);

            var html = await this.RenderViewAsync("Small/_Notes", notes, true);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                html = $"<div id='notes-section' hx-swap-oob='true'>{html}</div>",
                type = result.ResultType,
            });
        }

    }
}
