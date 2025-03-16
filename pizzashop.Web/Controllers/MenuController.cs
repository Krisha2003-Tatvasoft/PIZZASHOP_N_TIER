using AuthenticationDemo.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using VMCategory = pizzashop.Entity.ViewModels.Category;


namespace pizzashop.Controllers;

[CustomAuthorize]
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
    public async Task<IActionResult> CategoryList()
    {
        List<Category> categories = await _categoryService.GetCategoryList();
        return PartialView("_CategoryTable", categories);
    }

    [HttpGet]
    public async Task<IActionResult> itemTable(int id)
    {
        List<ItemTable> items = await _itemService.GetItemTable(id);
        return   PartialView("_ItemTable", items);
    }


    [HttpGet]
    public  async Task<IActionResult> ModiGroupList()
    {
        List<ModifiersGroup> modifiersGrops = await  _modifierGroupService.GetMGList();
        return PartialView("_ModifiersGroup", modifiersGrops);
    }

    [HttpGet]
    public async Task<IActionResult> modifierTable(int id)
    {
        List<ModifierTable> modifiers = await _modifierService.GetModifiersTable(id);
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
            return Json(new { sucess = true, message = "category edited succesfully" });
        }
        else
        {
            return Json(new { error = true, message = "error in edit category" });
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

    [HttpGet]
    public async Task<IActionResult> AddItem()
    {
        return PartialView("_Additem" ,await _itemService.Additem());
    }

    [HttpPost]
    public async Task<IActionResult> addItemPost(AddItem model)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
         if(await _itemService.AddItemPost(user.Userid,model))
         {
             return  Json(new { sucess = true, message = "Item Added Sucessfully" });
         }
         else
         {
             return Json(new { error = true, message = "Error in add item" });
         }
    }

     [HttpGet]
    public async Task<IActionResult> EditItem(int id)
    {
        return PartialView("_Edititem" ,await _itemService.EditItem(id));
    }

   [HttpPost]
   public async Task<IActionResult> EditItemPost(AddItem model)
   {
      CookieData user = SessionUtils.GetUser(HttpContext);
      if(await _itemService.EditItemPost(user.Userid,model))
      {
           return Json(new { success = true, message = "Item Added Sucessfully" });
      }
      else
      {
          return Json(new { error = true, message = "Error in update item" });
      }
   }

   
    [HttpPost]
    public async Task<IActionResult> DeleteItem(int id)
    {
        if (await _itemService.DeleteItem(id))
        {
            return Json(new { sucess = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "Item not deleted." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedItem(List<int> selectedIds)
    {
        
        
        if (await _itemService.DeleteSelectedItem(selectedIds))
        {
            return Json(new { sucess = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { error = true, message = "Item not deleted." });
        }
        
    }


    [HttpGet]
    public async Task<IActionResult> modifierList(int id)
    {
        List<ModifierList> modifiers = await _modifierService.GetModifiersList(id);
        
        return Ok(modifiers);
    }

}