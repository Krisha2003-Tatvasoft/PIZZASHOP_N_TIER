using pizzashop.Entity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace pizzashop.Repository.Interfaces;

public interface ICountryRepository
{
      
      Task<List<SelectListItem>> GetAllCountryAsync();
}
