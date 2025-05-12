using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;
using pizzashop.Web.Hubs;

namespace pizzashop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;

    private readonly ITableService _tableService;

    private readonly IHubContext<ChatHub> _hubContext;


    public WaitingListController(ISectionService sectionService,
    IOrderAppWaitingTokenService orderAppWaitingTokenService, ITableService tableService, IHubContext<ChatHub> hubContext)
    {
        _sectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;
        _tableService = tableService;
        _hubContext = hubContext;
    }

    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> WaitingListAsync()
    {
        return View();
    }

    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> WaitingSectionList()
    {
        var sectionList = await _sectionService.GetSectionWithCount();

        return PartialView("_WaitingSectionList", sectionList);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> WaitingTableList(int sectionid)
    {
        var tableList = await _orderAppWaitingTokenService.WaitingList(sectionid);
        return PartialView("_WaitingTableList", tableList);
    }


    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> EditWT(int id)
    {
        var WT = await _orderAppWaitingTokenService.EditGetWT(id);
        return PartialView("_EditWT", WT);
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> EditWTPost(AddWaitingToken model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            bool isthis = await _orderAppWaitingTokenService.EditPosttWT(user.Userid, model);

            if (isthis)
            {
                 // Call the SignalR hub to send a message
                await _hubContext.Clients.All.SendAsync("WTListUpdated");
                return Json(new { success = true, message = "Waiting Token Updated Sucessfully." });
            }
            else
            {
                return Json(new { success = false, message = "You Can Use This Email." });
            }
        }
        else
        {

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
            // // If model is invalid, return the same view with validation messages
            // return Json(new { message = "Validation Error." });
            // // return PartialView("_AddTable", model);
        }
    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> DeleteWT(int id)
    {
        if (await _orderAppWaitingTokenService.DeleteWT(id))
        {
            // Call the SignalR hub to send a message
             await _hubContext.Clients.All.SendAsync("WTListUpdated");
            return Json(new { success = true, message = "WaitingToken deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "WaitingToken not deleted." });
        }
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AddWaitingToken()
    {
        return PartialView("_AddWaitingToken", await _orderAppWaitingTokenService.AddWaitingToken(0));
    }



    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<JsonResult> GetSection()
    {

        var section = await _sectionService.GetSectionListDD();
        return Json(section);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<JsonResult> GetTable(int sectioid)
    {

        var table = await _tableService.GetTablesListDD(sectioid);
        return Json(table);
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AssignTable()
    {
        return PartialView("_AssignTableModel");
    }


    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AssignTablePost(WaitingListAssignTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            var (Orderid, message) = await _orderAppWaitingTokenService.AssignTablePost(user.Userid, model);

            if (Orderid != null)
            {
                 // Call the SignalR hub to send a message
             await _hubContext.Clients.All.SendAsync("AssignTable");
                return Json(new
                {
                    success = true,
                    orderid = Orderid,
                    message = message
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = message
                });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }


}
