namespace pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;

public interface ISectionService
{
    Task<List<VMSection>> GetSectionList();

    Task<bool> AddSectionPost(int loginId, VMSection viewmodel);

    Task<VMSection> EditSection(int id);

    Task<bool> EditSectionPost(int loginId, VMSection viewmodel);

     Task<bool> DeleteSection(int id);
}
