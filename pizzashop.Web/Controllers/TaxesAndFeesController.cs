using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;

public class TaxesAndFeesController : Controller
{
    private readonly ITaxesService _taxesService;

    public TaxesAndFeesController(ITaxesService taxesService)
    {
        _taxesService = taxesService;
    }


    [HttpGet]
    public async Task<IActionResult> TaxesAndFees(int page = 1, int pageSize = 5, string search = "")
    {
        var (taxList, totalTaxes) = await _taxesService.GetTaxTable(page, pageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalTaxes;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalTaxes / pageSize);

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_TaxList", taxList)
                  : View(taxList);
    }

    [HttpGet]
    public async Task<IActionResult> AddTax()
    {
        return PartialView("_AddTax");
    }

    [HttpPost]
    public async Task<IActionResult> AddTaxPost(AddTax model)
    {
        if (ModelState.IsValid)
        {

            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _taxesService.AddTaxPost(user.Userid, model))
            {
                return Json(new { success = true, message = "tax added Sucessfully." });
            }
            else
            {
                return Json(new { success = false, message = "tax not added." });
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
    public async Task<IActionResult> EditTax(int id)
    {
        return PartialView("_EditTax", await _taxesService.EditTax(id));
    }

    [HttpPost]
    public async Task<IActionResult> EditTaxPost(AddTax model)
    {
        if (ModelState.IsValid)
        {

            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _taxesService.EditTaxPost(user.Userid, model))
            {
                return Json(new { success = true, message = "tax Updated Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error in update tax" });
            }
        }
        else
        {
            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteTax(int id)
    {
        if (await _taxesService.DeleteTax(id))
        {
            return Json(new { success = true, message = "tax deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "tax not deleted." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEnable(int id, bool enable)
    {
       CookieData user = SessionUtils.GetUser(HttpContext);
        if (await _taxesService.UpdateEnable(user.Userid, id ,enable ))
        {
            return Json(new { success = true, message = "Enable Updated Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Enable Tax not Updated." });
        }
    }



}
