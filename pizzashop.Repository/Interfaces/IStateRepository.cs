namespace pizzashop.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
public interface IStateRepository
{
    Task<List<SelectListItem>> GetStatesByCountryAsync(int countryId);
}
