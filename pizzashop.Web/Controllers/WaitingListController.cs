using Microsoft.AspNetCore.Mvc;

namespace pizzashop.Web.Controllers;

public class WaitingListController : Controller
{
    public IActionResult WaitingList()
    {
        return View();
    }
}
