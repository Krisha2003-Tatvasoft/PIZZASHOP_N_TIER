using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using pizzashop.web.Attributes;

namespace pizzashop.Web.Controllers;

public class MenuOrderAppController : Controller
{
    private readonly ICategoryService _categoryService;

    private readonly IItemService _itemService;

    private readonly IOrderAppMenuService _orderAppMenuService;

    private readonly IOrderService _orderService;

    public MenuOrderAppController(ICategoryService categoryService, IItemService itemService, IOrderAppMenuService orderAppMenuService, IOrderService orderService)
    {
        _categoryService = categoryService;
        _itemService = itemService;
        _orderAppMenuService = orderAppMenuService;
        _orderService = orderService;
    }

    public IActionResult MenuOrders()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CategoryList()
    {
        return PartialView("_CategoryList", await _categoryService.GetMenuCategoryList());
    }


    [HttpGet]
    public async Task<IActionResult> ItemList(int id, string search = "")
    {
        var itemlist = await _itemService.GetMenuItem(id, search);

        return PartialView("_ItemList", itemlist);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavourite(int id)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        bool isFavourite = await _itemService.ToggleFavourite(user.Userid, id);
        return Json(new { isFavourite = isFavourite });

    }

    [HttpGet]
    public async Task<IActionResult> ModifierList(int id)
    {
        List<IMGMviewmodel> modifierslist = await _orderAppMenuService.ModifiersById(id);
        if (!modifierslist.Any())
        {
            return Json(new { success = false, message = "No modifiers For this Item." });
        }
        else
        {
            return PartialView("_ModifierList", modifierslist);
        }

    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
         var orderDetails = await _orderAppMenuService.OrderDetails(id); // Fetch order details by ID

        return PartialView("_OrderDetails", orderDetails);
    }


}
