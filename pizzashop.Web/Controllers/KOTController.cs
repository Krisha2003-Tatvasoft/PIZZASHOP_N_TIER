using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.web.Attributes;

namespace pizzashop.Web.Controllers;

public class KOTController : Controller
{
    private readonly ICategoryService _categoryService;

    private readonly IKOTService _KOTService;

    public KOTController(ICategoryService categoryService, IKOTService kOTService)
    {
        _categoryService = categoryService;
        _KOTService = kOTService;
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public IActionResult KOT()
    {
        return View();
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> CategoryList()
    {
        var CategoryList = await _categoryService.GetKOTCategoryList();
        return PartialView("_CategoryList", CategoryList);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> loadTickets(int id, string status, int page = 1)
    {
        var (tickets, totalOrder) = await _KOTService.Ticket(id, status, page);

        ViewBag.CurrentPage = page;
        ViewBag.totalOrder = totalOrder;

        return PartialView("_Ticket", tickets);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> OpenModel(int id, string status)
    {
        Ticket ticket = await _KOTService.TicketDetails(id, status);

        return PartialView("_TransferModal", ticket);
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> UpdateItemStatus([FromBody] OrderItemStatus model)
    {
        if (model == null || model.Items == null || !model.Items.Any())
        {
            return BadRequest("Invalid input.");
        }

        await _KOTService.UpdateItemStatusAsync(model);
        return Ok(new { success = true });
    }



}
