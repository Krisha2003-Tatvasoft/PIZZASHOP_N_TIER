using Microsoft.AspNetCore.Mvc;

namespace pizzashop.Web.Controllers;

public class KOTController :Controller
{
    public IActionResult KOT()
    {
        return View();
    }

}
