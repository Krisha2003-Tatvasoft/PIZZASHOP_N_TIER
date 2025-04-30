using System.Diagnostics;
// using AuthenticationDemo.Attributes;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Web.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using System.Threading.Tasks;

namespace pizzashop.Web.Controllers;

[CustomAuthorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IDashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;

    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(DateTime? fromDate, DateTime? toDate)
    {
        var model = await _dashboardService.GetDashboardDataAsync(fromDate, toDate);
        return PartialView("_DashboardPartial" , model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
