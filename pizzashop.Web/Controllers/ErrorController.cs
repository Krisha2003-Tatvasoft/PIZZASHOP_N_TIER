using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;


namespace pizzashop.Web.Controllers;

public class ErrorController :Controller
{
     [Microsoft.AspNetCore.Mvc.Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var referer = Request.Headers["Referer"].ToString();
        // Set a proper error message based on the status code
        switch (statusCode)
        {
            case 404:
                ViewBag.ErrorMessage = "404";
                break;
            case 403:
                ViewBag.ErrorMessage = "Access Denied";
                break;
            default:
                ViewBag.ErrorMessage = "Something occured";
                break;
        }
        // Render NotFound view

        TempData["ErrorMessage"] = "This action is not accessible to this User.";
        if(!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }
        return View("~/Views/Error/NotFound.cshtml");
    }

}
