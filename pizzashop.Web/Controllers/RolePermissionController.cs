// using AuthenticationDemo.Attributes;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using System.Security.Claims;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class RolePermissionController : Controller
{
    private readonly IRolePerService _rolePerService;

    public RolePermissionController(IRolePerService rolePerService)
    {
        _rolePerService = rolePerService;
    }

    [HttpGet]
    [CustomAuthorize("RoleAndPermission", "View")]
    public async Task<IActionResult> Role(int id)
    {
        // if (id == 0)
        // {
        //     return View(await _rolePerService.GetAllRoleAsync());
        // }
        // else
        // {
        //       return PartialView("_PermissionRole", await _rolePerService.GetPerTablesAsync(id));
        // }
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ?( (id!=0) ? PartialView("_PermissionRole", await _rolePerService.GetPerTablesAsync(id)):
                  View(await _rolePerService.GetAllRoleAsync()))
                  : View(await _rolePerService.GetAllRoleAsync());
    }


    [HttpPost]
    [CustomAuthorize("RoleAndPermission", "AddEdit")]
    public async Task<IActionResult> UpdatePermissions([FromBody] List<RolePermission> updatedPermissions)
    {
        if (updatedPermissions == null || !updatedPermissions.Any())
        {
            return BadRequest("No data received");
        }

        if (await _rolePerService.UpdatePerAsync(updatedPermissions))
        {
             var rolename = User.FindFirst(ClaimTypes.Role)?.Value;

            var allPermissions = await _rolePerService.GetPermissionById(rolename);

            var Permissions = allPermissions.Select(p => new RolePermission
            {
               Moduleid = p.Module.Moduleid,
                Canview = p.Canview,
                Canaddedit = p.Canaddedit,
                Candelete = p.Candelete,
                Rolename = p.Role.Rolename
            }).ToList();

            Response.Cookies.Delete("PermissionData");
            CookieUtils.SavePermissionData(Response, Permissions);

            return Ok("Permissions updated successfully");
        }
        else
        {
            return BadRequest("No data received");
        }
           
    }


}
