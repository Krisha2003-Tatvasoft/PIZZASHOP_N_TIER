using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;

public class MenuOrderAppController : Controller
{
    private readonly ICategoryService _categoryService;

    private readonly IItemService _itemService;

    public MenuOrderAppController(ICategoryService categoryService,IItemService itemService)
    {
        _categoryService = categoryService;
        _itemService = itemService;

    }

    public IActionResult MenuOrders()
    {
        return View();
    }

    public async Task<IActionResult> CategoryList()
    {
        return PartialView("_CategoryList", await _categoryService.GetMenuCategoryList());
    }

    public async Task<IActionResult> ItemList(int id, string search = "")
    {
        var itemlist = await _itemService.GetMenuItem(id,search);

        return PartialView("_ItemList", itemlist);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavourite(int id)
    {
         CookieData user = SessionUtils.GetUser(HttpContext);
        bool isFavourite = await _itemService.ToggleFavourite(user.Userid,id);
        return Json(new { isFavourite = isFavourite});
        
    }




}
