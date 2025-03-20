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
    public async Task<IActionResult> TablesList(int id)
    {
        return PartialView("_TablesTable", await _tableService.GetTableBySec(id));
    }

    [HttpGet]
    public async Task<IActionResult> AddSectionGet()
    {
        return PartialView("_AddSection");
    }


    [HttpPost]
    public async Task<IActionResult> AddSectionPost(VMSection model)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        if (await _sectionService.AddSectionPost(user.Userid, model))
        {
            return Json(new { success = true, message = "ModifierGroup deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Item not deleted." });
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
        CookieData user = SessionUtils.GetUser(HttpContext);
        if (await _tableService.AddTablePost(user.Userid, model))
        {
            return Json(new { success = true, message = "Table Added Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Table not Added." });
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
        CookieData user = SessionUtils.GetUser(HttpContext);
        if (await _tableService.EditTablePost(user.Userid,model))
        {
            return Json(new { success = true, message = "Table Updated Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Error in update table." });
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

}
