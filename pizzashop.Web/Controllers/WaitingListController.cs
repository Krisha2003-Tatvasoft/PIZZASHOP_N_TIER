using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class WaitingListController : Controller
{
   private readonly ISectionService _sectionService;

     private readonly IOrderAppWaitingTokenService _orderAppWaitingTokenService;


   public WaitingListController(ISectionService sectionService, 
   IOrderAppWaitingTokenService orderAppWaitingTokenService)
   {
      _sectionService = sectionService;
        _orderAppWaitingTokenService = orderAppWaitingTokenService;
   }

   public async Task<IActionResult> WaitingListAsync()
   {
      return View();
   }

   public async Task<IActionResult> WaitingSectionList()
   {
      var sectionList = await _sectionService.GetSectionWithCount();

      return PartialView("_WaitingSectionList", sectionList);
   }

   [HttpGet]
   public async Task<IActionResult> WaitingTableList(int sectionid)
   {
      var tableList = await _orderAppWaitingTokenService.WaitingList(sectionid);
      return PartialView("_WaitingTableList", tableList);
   }

}
