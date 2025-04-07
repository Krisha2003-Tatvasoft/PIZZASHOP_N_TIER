using Microsoft.AspNetCore.Mvc;

namespace pizzashop.Web.Controllers;

public class MenuOrderApp : Controller
{
    public IActionResult MenuOrders()
    {
        return View();
    }
}
