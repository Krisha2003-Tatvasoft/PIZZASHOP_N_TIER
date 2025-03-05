using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Controllers;

public class MenuController : Controller
{

    private readonly ICategoryService _categoryService;

    private readonly IItemService _itemService;

    private readonly IModifiersGroupService _modifierGroupService;

    private readonly IModifierService _modifierService;

    public MenuController(ICategoryService categoryService, IItemService itemService
    , IModifiersGroupService modifiersGroupService,IModifierService modifierService)
    {
        _categoryService = categoryService;
        _itemService = itemService;
        _modifierGroupService = modifiersGroupService;
        _modifierService = modifierService;
    }

    public IActionResult Menu()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> addCategory(Category model)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);

        await _categoryService.AddCategoryAsync(model, user.Userid);
        return Json(new { sucess = true, message = "wdcf" });
    }

    [HttpGet]
    public IActionResult CategoryList()
    {
        List<Category> categories = _categoryService.GetCategoryList();
        return PartialView("_CategoryTable", categories);
    }

    [HttpGet]
    public IActionResult itemTable(int id)
    {
        List<ItemTable> items = _itemService.GetItemTable(id);
        return PartialView("_ItemTable", items);
    }


    [HttpGet]
    public IActionResult ModiGroupList()
    {
        List<ModifiersGroup> modifiersGrops = _modifierGroupService.GetMGList();
        return PartialView("_ModifiersGroup", modifiersGrops);
    }

    [HttpGet]
    public IActionResult modifierTable(int id)
    {
        List<ModifierTable> modifiers = _modifierService.GetModifiersTable(id);
        return PartialView("_Modifier", modifiers);
    }

}