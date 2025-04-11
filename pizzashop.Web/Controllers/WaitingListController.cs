using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly ISectionService _sectionService;

    private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;




    public WaitingListController(ISectionService sectionService,
    IOrderAppWaitingTokenService orderAppWaitingTokenService)
    {
        _sectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;

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

    [HttpGet]
    public async Task<IActionResult> EmailExist(string email, int customId)
    {
        if (await _orderAppWaitingTokenService.EmailExistsWithId(email, customId))
        {
            return Json(new { success = true, message = "email already exist." });
        }
        else
        {
            return Json(new { success = false, message = "email not exists." });
        }
    }


    public async Task<IActionResult> EditWTPost(AddWaitingToken model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
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
    public async Task<IActionResult> AssignTable(int id)
    {
       return PartialView("_AssignTableModel");
    }






}
