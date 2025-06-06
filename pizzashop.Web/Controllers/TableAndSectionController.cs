using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;
using pizzashop.Web.Hubs;
using VMSection = pizzashop.Entity.ViewModels.Section;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class TableAndSectionController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly ITableService _tableService;

    private readonly IHubContext<ChatHub> _hubContext;

    public TableAndSectionController(ISectionService sectionService, ITableService tableService, IHubContext<ChatHub> hubContext)
    {
        _sectionService = sectionService;
        _tableService = tableService;
        _hubContext = hubContext;
    }

    public IActionResult TableAndSection()
    {
        return View();
    }

    [HttpGet]
    [CustomAuthorize("TableAndSection", "View")]
    public async Task<IActionResult> SectionList()
    {
        return PartialView("_SectionTable", await _sectionService.GetSectionList());
    }

    [HttpGet]
    [CustomAuthorize("TableAndSection", "View")]
    public async Task<IActionResult> TablesList(int id, int page = 1, int pageSize = 5, string search = "")
    {

        var (tables, totalTables) = await _tableService.GetTableBySec(id, page, pageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalTables = totalTables;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalTables / pageSize);

        return PartialView("_TablesTable", tables);
    }

    [HttpGet]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> AddSectionGet()
    {
        return PartialView("_AddSection");
    }


    [HttpPost]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> AddSectionPost(VMSection model)
    {
        if (ModelState.IsValid)
        {

            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _sectionService.AddSectionPost(user.Userid, model))
            {
                return Json(new { success = true, message = "section added Sucessfully." });
            }
            else
            {
                return Json(new { success = false, message = "section not added." });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpGet]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> EditSection(int id)
    {
        return PartialView("_EditSection", await _sectionService.EditSection(id));
    }

    [HttpPost]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> EditSectionPost(VMSection model)
    {

        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _sectionService.EditSectionPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Modifier Group Updated Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error in update modifiergroup" });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpPost]
    [CustomAuthorize("TableAndSection", "Delete")]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var (sucess, message) = await _sectionService.DeleteSection(id);
        if (sucess == true)
        {
            return Json(new { success = true, message = message });
        }
        else
        {
            return Json(new { success = false, message = message });
        }
    }


    [HttpGet]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> AddTable(int sectionId)
    {
        return PartialView("_AddTable", await _tableService.AddTable(sectionId));
    }

    [HttpPost]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> AddTablePost(AddTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _tableService.AddTablePost(user.Userid, model))
            {
                // Call the SignalR hub to send a message
                await _hubContext.Clients.All.SendAsync("TableAdded");
                return Json(new { success = true, message = "Table Added Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Table not Added." });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }


    [HttpGet]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> EditTable(int id)
    {
        return PartialView("_EditTable", await _tableService.EditTable(id));
    }


    [HttpPost]
    [CustomAuthorize("TableAndSection", "AddEdit")]
    public async Task<IActionResult> EditTablePost(AddTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _tableService.EditTablePost(user.Userid, model))
            {
                // Call the SignalR hub to send a message
                await _hubContext.Clients.All.SendAsync("TableUpdated");
                return Json(new { success = true, message = "Table Updated Sucessfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error in update table." });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
        }
    }

    [HttpPost]
    [CustomAuthorize("TableAndSection", "Delete")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        if (await _tableService.DeleteTable(id))
        {
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("TableDeleted");
            return Json(new { success = true, message = "Table deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Table is Currenty Occupied You can't delete that." });
        }
    }

    [HttpPost]
    [CustomAuthorize("TableAndSection", "Delete")]
    public async Task<IActionResult> DeleteSelectedTables(List<int> selectedTableIds)
    {
        if (await _tableService.DeleteSelectedTable(selectedTableIds))
        {
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("TableDeleted");
            return Json(new { sucess = true, message = "Table deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "Table not deleted." });
        }

    }


    [HttpPost]
    [CustomAuthorize("TableAndSection", "View")]
    public async Task<IActionResult> SaveOrderSection([FromBody] List<int> order)
    {
        if (order == null || !order.Any())
        {
            return BadRequest("Invalid order data.");
        }

        bool isSuccess = await _sectionService.SaveOrderSection(order);


        if (isSuccess)
        {
            return Json(new { success = true, message = "Section ordered Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Section not ordered." });
        }

    }

    [HttpGet]
    [CustomAuthorize("TableAndSection", "View")]
    public async Task<IActionResult> FirstSecId()
    {
        List<VMSection> sections = await _sectionService.GetSectionList();
        var firstSec = sections.FirstOrDefault();
        if (firstSec == null)
        {
            return Ok();
        }
        return Ok(firstSec.Sectionid);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTableIds(int sectionId)
    {
        var ids = await _tableService.GetAllTableIds(sectionId); // should return List<string> or List<int>
        return Json(ids);
    }

}
