using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;

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

    public IActionResult KOT()
    {
        return View();
    }

    public async Task<IActionResult> CategoryList()
    {
        var CategoryList = await _categoryService.GetKOTCategoryList();
        return PartialView("_CategoryList", CategoryList);
    }

    [HttpGet]
    public async Task<IActionResult> loadTickets(int id, string status, int page = 1)
    {
        var (tickets, totalOrder) = await _KOTService.Ticket(id, status, page);

        ViewBag.CurrentPage = page;
        ViewBag.totalOrder = totalOrder;

        return PartialView("_Ticket", tickets);
    }

    [HttpGet]
    public async Task<IActionResult> OpenModel(int id, string status)
    {
        Ticket ticket = await _KOTService.TicketDetails(id, status);

        return PartialView("_TransferModal", ticket);
    }

    [HttpPost]
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
