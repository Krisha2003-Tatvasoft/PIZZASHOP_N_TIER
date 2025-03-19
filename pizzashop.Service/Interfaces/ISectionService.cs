namespace pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;

public interface ISectionService
{
    Task<List<VMSection>> GetSectionList();
}
