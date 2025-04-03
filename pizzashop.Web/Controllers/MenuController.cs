// using AuthenticationDemo.Attributes;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using VMCategory = pizzashop.Entity.ViewModels.Category;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


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


    [CustomAuthorize("Menu", "View")]
    public IActionResult Menu()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> addCategory(VMCategory category)
    {
        if (ModelState.IsValid)
        {
            VMCategory model = category;
            CookieData user = SessionUtils.GetUser(HttpContext);

            if (await _categoryService.AddCategoryAsync(model, user.Userid))
            {
                return Json(new { success = true, message = "Category Added sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "category not Added." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpGet]
    public async Task<IActionResult> CategoryList()
    {
        List<VMCategory> categories = await _categoryService.GetCategoryList();
        return PartialView("_CategoryTable", categories);
    }

    [HttpGet]
    public async Task<IActionResult> itemTable(int id, int page = 1, int pageSize = 5, string search = "")
    {
        var (items, totalitem) = await _itemService.GetItemTable(id, page, pageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalitem = totalitem;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalitem / pageSize);

        return PartialView("_ItemTable", items);
    }


    [HttpGet]
    public async Task<IActionResult> ModiGroupList()
    {
        List<ModifiersGroup> modifiersGrops = await _modifierGroupService.GetMGList();
        return PartialView("_ModifiersGroup", modifiersGrops);
    }

    [HttpGet]
    public async Task<IActionResult> modifierTable(int id, int page = 1, int pageSize = 5, string search = "")
    {
        var (modifiers, totalMoidifier) = await _modifierService.GetModifiersTable(id, page, pageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalMoidifier = totalMoidifier;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalMoidifier / pageSize);

        return PartialView("_Modifier", modifiers);
    }

    [HttpGet]
    public async Task<IActionResult> EditCat(int id)
    {

        VMCategory category = await _categoryService.GetCategoryById(id);
        return Json(category);
    }

    [HttpPost]
    public async Task<IActionResult> EditCat([FromBody] VMCategory model)
    {
        if (ModelState.IsValid)
        {
            if (await _categoryService.UpdateCat(model))
            {
                return Json(new { success = true, message = "category edited succesfully" });
            }
            else
            {
                return Json(new { success = false, message = "category not edited." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteCat(int id)
    {
        if (await _categoryService.DeleteCat(id))
        {
            return Json(new { sucess = true, message = "category deleted succesfully" });
        }
        else
        {
            return Json(new { success = false, message = "error in delete category" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddItem()
    {
        return PartialView("_Additem", await _itemService.Additem());
    }

    [HttpPost]
    public async Task<IActionResult> addItemPost(AddItem model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _itemService.AddItemPost(user.Userid, model) == true)
            {
                return Json(new { success = true, message = "Item Added Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "category not Added." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpGet]
    public async Task<IActionResult> EditItem(int id)
    {
        return PartialView("_Edititem", await _itemService.EditItem(id));
    }

    [HttpPost]
    public async Task<IActionResult> EditItemPost(AddItem model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _itemService.EditItemPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Item Added Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "item not edited." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
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
            return Json(new { success = false, message = "Item not deleted." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedItem(List<int> selectedIds)
    {


        if (await _itemService.DeleteSelectedItem(selectedIds))
        {
            return Json(new { success = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Item not deleted." });
        }

    }


    [HttpGet]
    public async Task<IActionResult> modifierList(int id)
    {
        List<ModifierList> modifiers = await _modifierService.GetModifiersList(id);

        return Ok(modifiers);
    }


    [HttpGet]
    public async Task<IActionResult> AddModifier()
    {
        return PartialView("_AddModifier", await _modifierService.AddModifier());
    }

    [HttpPost]
    public async Task<IActionResult> addModifierPost(AddModifier model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _modifierService.AddModifierPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Modifier Added Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "modifier not added." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpGet]
    public async Task<IActionResult> EditModifier(int id)
    {
        var data = await _modifierService.EditModifier(id);
        string partialViewHtml = await RenderPartialViewToString("_EditModifier", data);
        return Json(new
        {
            selectedMGIds = data.SelectedMGIds,
            html = partialViewHtml   // Return the rendered partial view
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditModifietrPost(AddModifier model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _modifierService.EditModifierPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Modifier Updated Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "modifier not edited." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }


    }

    [HttpPost]
    public async Task<IActionResult> DeleteModifier(int id, int MGId)
    {
        if (await _modifierService.DeleteModifier(id, MGId))
        {
            return Json(new { success = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Item not deleted." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedModifiers(List<int> selectedIds, int MGId)
    {


        if (await _modifierService.DeleteSelectedModifier(selectedIds, MGId))
        {
            return Json(new { success = true, message = "Item deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Item not deleted." });
        }

    }

    [HttpGet]
    public async Task<IActionResult> FirstMGId()
    {
        List<ModifiersGroup> modifiersGrops = await _modifierGroupService.GetMGList();
        var firstMG = modifiersGrops.FirstOrDefault();
        if (firstMG == null)
        {
            return Ok();
        }
        return Ok(firstMG.Modifiergroupid);
    }


    public async Task<IActionResult> FirstCatId()
    {
        List<VMCategory> categories = await _categoryService.GetCategoryList();
        var firstCat = categories.FirstOrDefault();
        if (firstCat == null)
        {
            return Ok();
        }
        return Ok(firstCat.Categoryid);
    }


    [HttpGet]
    public async Task<IActionResult> AddMG()
    {
        return PartialView("_AddModifierGroup");
    }


    [HttpPost]
    public async Task<IActionResult> AddMGPost(AddModifierGroup model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _modifierGroupService.AddMGPost(user.Userid, model))
            {
                return Json(new { success = true, message = "ModifierGroup Added Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "modifiergroup not added." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    private async Task<string> RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;

        using (var writer = new StringWriter())
        {
            var viewEngine = HttpContext.RequestServices.GetService<ICompositeViewEngine>();
            var viewResult = viewEngine?.FindView(ControllerContext, viewName, false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"View '{viewName}' not found.");
            }

            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditMG(int id)
    {
        var data = await _modifierGroupService.EditMG(id);
        string partialViewHtml = await RenderPartialViewToString("_EditModifierGroup", data);

        return Json(new
        {
            selectedModifiers = data.SelectedModifiers,
            html = partialViewHtml   // Return the rendered partial view
        });


    }

    [HttpPost]
    public async Task<IActionResult> EditMGPost(AddModifierGroup model)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            if (await _modifierGroupService.EditMGPost(user.Userid, model))
            {
                return Json(new { success = true, message = "Modifier Group Updated Sucessfully" });
            }
            else
            {
                return Json(new { success = false, message = "modifiergroup not edited." });
            }
        }
        else
        {

            // If model is invalid, return the same view with validation messages
            return Json(new { message = "Validation Error." });
            // return PartialView("_AddTable", model);
        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteMG(int id)
    {
        if (await _modifierGroupService.DeleteMG(id))
        {
            return Json(new { success = true, message = "ModifierGroup deleted Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "ModifierGroup not deleted." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> SelectExModifier(int page = 1, int pageSize = 5, string search = "")
    {
        var (Exmodifiers, totalExMoidifier) = await _modifierService.GetAllModifier(page, pageSize, search);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.totalExMoidifier = totalExMoidifier;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalExMoidifier / pageSize);

        return PartialView("_SelectExistingModifier", Exmodifiers);
    }

    [HttpPost]
    public async Task<IActionResult> SaveOrderCategory([FromBody] List<int> order)
    {
        if (order == null || !order.Any())
        {
            return BadRequest("Invalid order data.");
        }

        bool isSuccess = await _categoryService.SaveOrderCategory(order);


        if (isSuccess)
        {
            return Json(new { success = true, message = "category ordered Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "category not ordered." });
        }

    }

    [HttpPost]
    public async Task<IActionResult> SaveOrderMg([FromBody] List<int> order)
    {
        if (order == null || !order.Any())
        {
            return BadRequest("Invalid order data.");
        }

        bool isSuccess = await _modifierGroupService.SaveOrderMG(order);


        if (isSuccess)
        {
            return Json(new { success = true, message = "ModifierGroup ordered Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "ModifierGroup not ordered." });
        }

    }

    [HttpPost]
    public async Task<IActionResult> UpdateItemAvailble(int id, bool available)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        if (await _itemService.UpdateAvailable(user.Userid, id, available))
        {
            return Json(new { success = true, message = "Available item Updated Sucessfully." });
        }
        else
        {
            return Json(new { success = false, message = "Available item not Updated." });
        }
    }



}