using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;

    private readonly ITableService _tableService;




    public WaitingListController(ISectionService sectionService,
    IOrderAppWaitingTokenService orderAppWaitingTokenService, ITableService tableService)
    {
        _sectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;
        _tableService = tableService;

    }

    public async Task<IActionResult> WaitingListAsync()
    {
        return View();
    }

    public async Task<IActionResult> WaitingSectionList()
    {
        var sectionList = await _sectionService.GetSectionWithCount();

        return PartialView("_WaitingSectionList", sectionList);
    }

    [HttpGet]
    public async Task<IActionResult> WaitingTableList(int sectionid)
    {
        var tableList = await _orderAppWaitingTokenService.WaitingList(sectionid);
        return PartialView("_WaitingTableList", tableList);
    }


    [HttpGet]
    public async Task<IActionResult> EditWT(int id)
    {
        var WT = await _orderAppWaitingTokenService.EditGetWT(id);
        return PartialView("_EditWT", WT);
    }

    // [HttpGet]
    // public async Task<IActionResult> EmailExist(string email, int customId)
    // {
    //     if (await _orderAppWaitingTokenService.EmailExistsWithId(email, customId))
    //     {
    //         return Json(new { success = true, message = "email already exist." });
    //     }
    //     else
    //     {
    //         return Json(new { success = false, message = "email not exists." });
    //     }
    // }


    public async Task<IActionResult> EditWTPost(AddWaitingToken model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            bool isthis = await _orderAppWaitingTokenService.EditPosttWT(user.Userid, model);

            if (await _orderAppWaitingTokenService.EditPosttWT(user.Userid, model))
            {
                return Json(new { success = true, message = "token Updated Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error in update token" });
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
    public async Task<IActionResult> DeleteWT(int id)
    {
        if (await _orderAppWaitingTokenService.DeleteWT(id))
        {
            return Json(new { success = true, message = "WaitingToken deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "WaitingToken not deleted." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddWaitingToken()
    {
        return PartialView("_AddWaitingToken", await _orderAppWaitingTokenService.AddWaitingToken(0));
    }



    [HttpGet]
    public async Task<JsonResult> GetSection()
    {

        var section = await _sectionService.GetSectionListDD();
        return Json(section);
    }

    [HttpGet]
    public async Task<JsonResult> GetTable(int sectioid)
    {

        var table = await _tableService.GetTablesListDD(sectioid);
        return Json(table);
    }

    [HttpGet]
    public async Task<IActionResult> AssignTable()
    {
        return PartialView("_AssignTableModel");
    }


    [HttpPost]
    public async Task<IActionResult> AssignTablePost(WaitingListAssignTable model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            var (Orderid, message) = await _orderAppWaitingTokenService.AssignTablePost(user.Userid, model);

            if (Orderid != null)
            {
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
