// using AuthenticationDemo.Attributes;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using System.Security.Claims;
using pizzashop.Service.Utils;
using Microsoft.AspNetCore.SignalR;
using pizzashop.Web.Hubs;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class RolePermissionController : Controller
{
    private readonly IRolePerService _rolePerService;

    private readonly IHubContext<ChatHub> _hubContext;

    public RolePermissionController(IRolePerService rolePerService, IHubContext<ChatHub> hubContext)
    {
        _rolePerService = rolePerService;
        _hubContext = hubContext;
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
                  ? ((id != 0) ? PartialView("_PermissionRole", await _rolePerService.GetPerTablesAsync(id)) :
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
            // Call the SignalR hub to send a message
            await _hubContext.Clients.All.SendAsync("Permission");
            return Ok("Permissions updated successfully");
        }
        else
        {
            return BadRequest("No data received");
        }

    }


    [HttpPost]
    public async Task UpdateperCookie()
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        Console.WriteLine("hiiiiiiiiiiiiiiiiiiiiiiiiiiiiii");
        var allPermissions = await _rolePerService.GetPermissionById(user.Rolename);

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
    }


}
