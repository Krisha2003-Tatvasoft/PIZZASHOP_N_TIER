using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class TaxesAndFeesController : Controller
{
     private readonly ITaxesService _taxesService;

     public TaxesAndFeesController(ITaxesService taxesService)
     {
        _taxesService= taxesService;
     }


    [HttpGet]
    public async Task<IActionResult> TaxesAndFees(int page = 1, int pageSize = 5, string search = "")
    {
        var (taxList, totalTaxes) = await _taxesService.GetTaxTable(page,pageSize,search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalTaxes;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalTaxes / pageSize);

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_TaxList",taxList)
                  : View(taxList);

    }

}
