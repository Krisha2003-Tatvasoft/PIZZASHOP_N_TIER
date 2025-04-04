using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class CustomerController :Controller
{
  private readonly ICustomerService _customerService;

  public readonly ICustomerExportService _customerExportService;


    public CustomerController(ICustomerService customerService, ICustomerExportService customerExportService)
    {
        _customerService = customerService;
        _customerExportService = customerExportService;
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

    
    [HttpGet]
    public async Task<IActionResult> ExportCustomer(string search = "", DateTime? fromDate = null, DateTime? toDate = null)
    {
        // Fetch orders based on filters
        List<CustomerTable> customers = await _customerService.GetExcelCustomer(search, fromDate, toDate);

        if(customers == null || !customers.Any())
        {
           return Json(new { success = false, message = "No data to export" });
        }
        // Generate Excel file
        var fileContent = _customerExportService.ExportCustomerToExcel(customers, search, fromDate, toDate);

        // Return as downloadable file
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
    }

      [HttpGet]
    public async Task<IActionResult> CustomerHistory(int id)
    {
       return PartialView("_CustomerHistory", await _customerService.CustomerHistory(id));
    }


}
