using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;

public class TableOrderAppController : Controller
{

    private readonly ISectionService _SectionService;

    private readonly ICustomerService _cutomerService;

    private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;

    private readonly IOrderAppTableService _orderAppTableService;

    public TableOrderAppController(ISectionService sectionService, IOrderAppWaitingTokenService orderAppWaitingTokenService
    , ICustomerService customerService, IOrderAppTableService orderAppTableService)
    {
        _SectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;
        _cutomerService = customerService;
        _orderAppTableService = orderAppTableService;
    }


    public async Task<IActionResult> TablesOrder()
    {
        var tableSectionData = await _SectionService.OrderTableViews();

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_TableSection", tableSectionData)
                  : View();
    }

    [HttpGet]
    public async Task<IActionResult> AddWaitingToken(int id)
    {
        return PartialView("_AddWaitingToken", await _orderAppWaitingTokenService.AddWaitingToken(id));
    }

    [HttpGet]
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
    public async Task<IActionResult> AddWTPost(AddWaitingToken model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _orderAppWaitingTokenService.AddWaitingTokenPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Waiting Token Added Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Waiting Token  not Added." });
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
    public async Task<IActionResult> GetWaitingListParial(int id)
    {
        return PartialView("_WaitingList", await _orderAppWaitingTokenService.WaitingListBySectionId(id));
    }


    [HttpGet]
    public async Task<IActionResult> AssignTable(int id)
    {
        return PartialView("_AssignTable", await _orderAppTableService.AssignTable(id));
    }

    [HttpGet]
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
    public async Task<IActionResult> assignTablePost(AssignTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            int? Orderid = await _orderAppTableService.AssignTablePost(user.Userid, model);

            return Json(new
            {
                success = true,
                orderid = Orderid,
            });
        }
        else
        {
            
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

}
