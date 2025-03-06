using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using VMCategory = pizzashop.Entity.ViewModels.Category;


namespace pizzashop.Controllers;

public class MenuController : Controller
{

    private readonly ICategoryService _categoryService;

    private readonly IItemService _itemService;

    private readonly IModifiersGroupService _modifierGroupService;

    private readonly IModifierService _modifierService;

    public MenuController(ICategoryService categoryService, IItemService itemService
    , IModifiersGroupService modifiersGroupService, IModifierService modifierService)
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
    public async Task<IActionResult> addCategory(MenuViewModels MenuVM)
    {
        VMCategory model = MenuVM.category;
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

    [HttpGet]
    public async Task<IActionResult> EditCat(int id)
    {
        VMCategory category = await _categoryService.GetCategoryById(id);
        return Json(category);
    }

    [HttpPost]
    public async Task<IActionResult> EditCat([FromBody] Category model)
    {
        if (await _categoryService.UpdateCat(model))
        {
            return Json(new { sucess = true, message = "wdcf" });
        }
        else
        {
            return Json(new { error = true, message = "wdcf" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCat(int id)
    {
        if (await _categoryService.DeleteCat(id))
        {
            return Json(new { sucess = true, message = "wdcf" });
        }
        else
        {
            return Json(new { error = true, message = "wdcf" });
        }

    }

}