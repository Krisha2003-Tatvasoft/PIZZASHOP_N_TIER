using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;
using pizzashop.Web.Hubs;

namespace pizzashop.Web.Controllers;

public class TableOrderAppController : Controller
{

    private readonly ISectionService _SectionService;

    private readonly ICustomerService _cutomerService;

    private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;

    private readonly IOrderAppTableService _orderAppTableService;

    private readonly IHubContext<ChatHub> _hubContext;


    public TableOrderAppController(ISectionService sectionService, IOrderAppWaitingTokenService orderAppWaitingTokenService
    , ICustomerService customerService, IOrderAppTableService orderAppTableService, IHubContext<ChatHub> hubContext)
    {
        _SectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;
        _cutomerService = customerService;
        _orderAppTableService = orderAppTableService;
        _hubContext = hubContext;
    }


    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public IActionResult TablesOrder(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            if (msg == "cancelled")
            {
                TempData["ErrorMessage"] = "This Order is Completed Or Canclled";
            }
        }

        return View();
    }



    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> TablePartial()
    {
        var tableSectionData = await _SectionService.OrderTableViews();

        return PartialView("_TableSection", tableSectionData);

    }


    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AddWaitingToken(int id)
    {
        return PartialView("_AddWaitingToken", await _orderAppWaitingTokenService.AddWaitingToken(id));
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var customers = await _cutomerService.GetCustomerByEmail(email);

        if (customers == null)
        {
            return Json(new { success = false });
        }

        return Json(new
        {
            success = true,
            customerName = customers.Customername,
            phoneNo = customers.Phoneno
        });


    }


    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AddWTPost(AddWaitingToken model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            var (success, message) = await _orderAppWaitingTokenService.AddWaitingTokenPost(user.Userid, model);
            if (success == true)
            {
                // Call the SignalR hub to send a message
                await _hubContext.Clients.All.SendAsync("WTListUpdated");
                return Json(new { success = true, message = message });
            }
            else
            {
                return Json(new { success = false, message = message });
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
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> GetWaitingListParial(List<int> id)
    {
        return PartialView("_WaitingList", await _orderAppWaitingTokenService.WaitingListBySectionId(id));
    }


    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> AssignTable(List<int> id)
    {
        return PartialView("_AssignTable", await _orderAppTableService.AssignTable(id));
    }

    [HttpGet]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> GetCustomerByWaitingToken(int Id)
    {

        var wt = await _orderAppWaitingTokenService.DetailsFromWT(Id);

        if (wt == null)
        {
            return Json(new { success = false });
        }

        return Json(new
        {
            success = true,
            customerName = wt.Customername,
            email = wt.Email,
            phoneNo = wt.Phoneno,
            noofperson = wt.Noofperson,
            waitingtokenid = wt.Waitingtokenid,
        });


    }

    [HttpPost]
    [CustomAuthorize("", "", new string[] { "Account Manager" })]
    public async Task<IActionResult> assignTablePost(AssignTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            var (Orderid, message) = await _orderAppTableService.AssignTablePost(user.Userid, model);
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

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
        }

    }

}
