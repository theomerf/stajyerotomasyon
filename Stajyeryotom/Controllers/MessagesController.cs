using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Contracts;
using System.Security.Claims;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IServiceManager _manager;

        public MessagesController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var messages = await _manager.MessageService.GetAllMessagesAsync();

                return PartialView("_Index", messages);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var messages = await _manager.MessageService.GetAllMessagesForOneUserAsync(userId!);

                return PartialView("_Index", messages);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMessage()
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

            return PartialView("_AddMessage");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddMessage([FromForm] MessageDtoForCreation messageDto)
        {
            var result = await _manager.MessageService.CreateMessageAsync(messageDto);

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
        public async Task<IActionResult> DeleteMessage([FromQuery] int messageId)
        {
            var result = await _manager.MessageService.DeleteMessageAsync(messageId);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                loadComponent = result.LoadComponent,
                type = result.ResultType,
            });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMessage([FromQuery] int messageId)
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

            var model = await _manager.MessageService.GetMessageForUpdateByIdAsync(messageId);

            ViewBag.DepartmentList = new SelectList(ViewBag.Departments, "DepartmentId", "DepartmentName", model?.DepartmentId);
            ViewBag.SelectedDepartmentId = model?.DepartmentId;
            ViewBag.SelectedSectionId = model?.SectionId;

            return PartialView("_UpdateMessage", model);
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateMessage([FromForm] MessageDtoForUpdate messageDto)
        {
            var result = await _manager.MessageService.UpdateMessageAsync(messageDto);
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
