using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.web.Attributes;
using pizzashop.Web.Hubs;

namespace pizzashop.Web.Controllers;

public class KOTController : Controller
{
    private readonly ICategoryService _categoryService;

    private readonly IKOTService _KOTService;

    private readonly IHubContext<ChatHub> _hubContext;


    public KOTController(ICategoryService categoryService, IKOTService kOTService, IHubContext<ChatHub> hubContext)
    {
        _categoryService = categoryService;
        _KOTService = kOTService;
        _hubContext = hubContext;
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public IActionResult KOT()
    {
        return View();
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> CategoryList(string status)
    {
        var CategoryList = await _categoryService.GetKOTCategoryList(status);
        return PartialView("_CategoryList", CategoryList);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager", "Chef" })]
    public async Task<IActionResult> loadTickets(int id, string status)
    {
        var tickets = await _KOTService.Ticket(id, status);

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
        // Call the SignalR hub to send a message
        await _hubContext.Clients.All.SendAsync("KOTUpdated");
        return Ok(new { success = true });
    }



}
