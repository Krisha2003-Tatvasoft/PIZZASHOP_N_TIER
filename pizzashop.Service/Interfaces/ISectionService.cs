namespace pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;
using VMOrderTableView = pizzashop.Entity.ViewModels.OrderTableView;
using VMSectionWIthCount = pizzashop.Entity.ViewModels.SectionWithCount;
using Microsoft.AspNetCore.Mvc.Rendering;

public interface ISectionService
{
    Task<List<VMSection>> GetSectionList();

    Task<bool> AddSectionPost(int loginId, VMSection viewmodel);

    Task<VMSection> EditSection(int id);

    Task<bool> EditSectionPost(int loginId, VMSection viewmodel);

     Task<bool> DeleteSection(int id);

     Task<bool> SaveOrderSection(List<int> orderIds);

    Task<List<VMOrderTableView>> OrderTableViews();

    Task<List<VMSectionWIthCount>> GetSectionWithCount();

    Task<List<SelectListItem>> GetSectionListDD();
}
