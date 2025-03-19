using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;
using pizzashop.web.Attributes;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class TableAndSectionController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly ITableService _tableService;

    public TableAndSectionController(ISectionService sectionService,ITableService tableService)
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



}
