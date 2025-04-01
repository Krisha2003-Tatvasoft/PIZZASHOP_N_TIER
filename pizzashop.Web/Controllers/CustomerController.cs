using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class CustomerController :Controller
{
  private readonly ICustomerService _customerService;


    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
  

   [HttpGet]
    public async Task<IActionResult> Customer(int page = 1, int pageSize = 5, string search = "", string SortColumn = "",
     string SortOrder = "", DateTime? fromDate = null, DateTime? toDate = null)
    {
        var (CustomerList, totalCustomer) = await _customerService.GetCustomerTable(page, pageSize, search, SortColumn, SortOrder,fromDate, toDate);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalCustomer = totalCustomer;
        ViewBag.SortColumn = SortColumn;
        ViewBag.SortOrder = SortOrder;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCustomer / pageSize);

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_CustomerTable", CustomerList)
                  : View(CustomerList);

    }


}
