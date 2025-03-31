using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;
using VMSection = pizzashop.Entity.ViewModels.Section;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class TableAndSectionController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly ITableService _tableService;

    public TableAndSectionController(ISectionService sectionService, ITableService tableService)
    {
        _sectionService = sectionService;
        _tableService = tableService;
    }

    public IActionResult TableAndSection()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SectionList()
    {
        return PartialView("_SectionTable", await _sectionService.GetSectionList());
    }

    [HttpGet]
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
    public async Task<IActionResult> AddSectionGet()
    {
        return PartialView("_AddSection");
    }


    [HttpPost]
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
    public async Task<IActionResult> EditSection(int id)
    {
        return PartialView("_EditSection", await _sectionService.EditSection(id));
    }

    [HttpPost]
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
    public async Task<IActionResult> DeleteSection(int id)
    {
        if (await _sectionService.DeleteSection(id))
        {
            return Json(new { success = true, message = "Section deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "Section not deleted." });
        }
    }


    [HttpGet]
    public async Task<IActionResult> AddTable()
    {
        return PartialView("_AddTable", await _tableService.AddTable());
    }

    [HttpPost]
    public async Task<IActionResult> AddTablePost(AddTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _tableService.AddTablePost(user.Userid, model))
            {
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
    public async Task<IActionResult> EditTable(int id)
    {
        return PartialView("_EditTable", await _tableService.EditTable(id));
    }


    [HttpPost]
    public async Task<IActionResult> EditTablePost(AddTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _tableService.EditTablePost(user.Userid, model))
            {
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
    public async Task<IActionResult> DeleteTable(int id)
    {
        if (await _tableService.DeleteTable(id))
        {
            return Json(new { sucess = true, message = "Table deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Table not deleted." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedTables(List<int> selectedTableIds)
    {


        if (await _tableService.DeleteSelectedTable(selectedTableIds))
        {
            return Json(new { sucess = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "Item not deleted." });
        }

    }


    [HttpPost]
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

}
