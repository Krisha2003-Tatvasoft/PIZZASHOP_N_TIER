using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;

namespace pizzashop.Web.Controllers;

public class LocationController : Controller
{
    private readonly IProfileService _ProfileService;

    public LocationController(IProfileService profileService)
    {
        _ProfileService = profileService;
    }
   
    [HttpGet]
    public async Task<JsonResult> GetStates(int countryId)
    {
       
        var states = await _ProfileService.GetStatesByCountryAsync(countryId);
        return Json(states);
    }

    [HttpGet]
    public async Task<JsonResult> GetCities(int stateId)
    {
        var cities =  await  _ProfileService.GetCitiesByStateAsync(stateId);
        return Json(cities);
    }
}
