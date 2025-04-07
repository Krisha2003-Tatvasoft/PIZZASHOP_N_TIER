using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class TableOrderAppController : Controller
{

     private readonly ISectionService _SectionService  ;

    public TableOrderAppController(ISectionService sectionService)
    {
        _SectionService = sectionService;
    }


    public async Task<IActionResult> TablesOrder()
    {
         var tableSectionData = await _SectionService.OrderTableViews();
        
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_TableSection" , tableSectionData)
                  : View();
    }
}
