using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.Qualified;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Implementations;
using pizzashop.Service.Interfaces;
using pizzashop.web.Attributes;



namespace pizzashop.Web.Controllers;

[CustomAuthorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    private readonly IOrderExportService _orderExportService;


    public OrdersController(IOrderService orderService, IOrderExportService orderExportService)
    {
        _orderService = orderService;
        _orderExportService = orderExportService;
       
    }

    [HttpGet]
    [CustomAuthorize("Order", "View")]
    public async Task<IActionResult> Orders(int page = 1, int pageSize = 5, string search = "", string SortColumn = "",
     string SortOrder = "", string status = "", DateTime? fromDate = null, DateTime? toDate = null)
    {
       
        var (orderList, totalOrder) = await _orderService.GetOrderTable(page, pageSize, search, SortColumn, SortOrder, status, fromDate, toDate);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalOrder = totalOrder;
        ViewBag.SortColumn = SortColumn;
        ViewBag.SortOrder = SortOrder;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrder / pageSize);

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_OrderTable", orderList)
                  : View(orderList);

    }

    [HttpGet]
    [CustomAuthorize("Order", "View")]
    public async Task<IActionResult> ExportOrders(string search = "", string status = "", DateTime? fromDate = null, DateTime? toDate = null)
    {
        // Fetch orders based on filters
        List<OrderTable> orders = await _orderService.GetExcelOrderTable(search, status, fromDate, toDate);

        if(orders == null || !orders.Any())
        {
           return Json(new { success = false, message = "No data to export" });
        }

        // Generate Excel file
        var fileContent = _orderExportService.ExportOrdersToExcel(orders, search, status, fromDate, toDate);

        // Return as downloadable file
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [HttpGet]
    [CustomAuthorize("Order", "View")]
    public async Task<IActionResult> OrdersDetails(int id)
    {
        return PartialView("_OrderDetails", await _orderService.OrderDetails(id));
    }



    [HttpGet]
    [CustomAuthorize("", "" , new string[] {"Admin" , "Account Manager"})]
    public async Task<IActionResult> orderInvoice(int id)
    {
        var orderDetails = await _orderService.OrderDetails(id); // Fetch order details by ID
    
        return PartialView("_OrderInvoice", orderDetails);
    }
    

}




