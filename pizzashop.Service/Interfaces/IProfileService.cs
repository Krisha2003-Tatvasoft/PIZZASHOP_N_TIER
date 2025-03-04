using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IProfileService
{
     Task ChangePassword(string email,ChangePassword model);

     Task<UserProfile?> UserProfile(string email);

     Task UpdateProfile(int id,UserProfile viewmodel);

     Task<List<SelectListItem>> GetStatesByCountryAsync(int countryId);

     Task<List<SelectListItem>> GetCitiesByStateAsync(int stateId);

}
