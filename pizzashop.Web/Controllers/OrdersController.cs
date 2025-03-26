using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class OrdersController :Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

   [HttpGet]
    public async Task<IActionResult> Orders(int page = 1, int pageSize = 5, string search = "", string SortColumn = "", 
    string SortOrder = "",string status ="",DateTime? fromDate=null,DateTime? toDate =null)
    {
        var (orderList, totalOrder) = await _orderService.GetOrderTable(page, pageSize, search, SortColumn, SortOrder,status,fromDate,toDate);

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

}
