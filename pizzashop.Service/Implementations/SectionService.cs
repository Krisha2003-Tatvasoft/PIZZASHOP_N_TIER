using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;
namespace pizzashop.Service.Implementations;

public class SectionService : ISectionService
{
     private readonly ISectionRepository _sectionRepository;

     public SectionService(ISectionRepository sectionRepository)
     {
        _sectionRepository= sectionRepository;
     }

    public async Task<List<VMSection>> GetSectionList()
    {
        var sectionList = await _sectionRepository.AllSections();

        var sections = sectionList.Select(c => new VMSection
        {
           Sectionid=c.Sectionid,
           Sectionname = c.Sectionname,
           Description = c.Description
        }).ToList();

        return sections;
    }

}
