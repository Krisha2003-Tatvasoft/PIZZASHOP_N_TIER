using Microsoft.AspNetCore.Mvc.Rendering;
namespace pizzashop.Repository.Interfaces;

public interface ICityRepository
{
    
      Task<List<SelectListItem>> GetCitiesByStateAsync(int stateId);
}
