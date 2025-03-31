using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.Entity.Models;
using VMSection = pizzashop.Entity.ViewModels.Section;

namespace pizzashop.Repository.Interfaces;

public interface ISectionRepository
{
    Task<List<Section>> AllSections();

    Task AddNewSection(Section section);

    Task<bool> SectionNameAsync(string sectioname);


    Task<bool> SecNameExistAtEditAsync(string sectionname, int id);

    Task<Section> SecByIdAsync(int id);

    Task UpdateSection(Section section);

    Task DeleteSection(Section section);

     Task<List<SelectListItem>> SectionDDAsync();

     Task<List<Section>> AllSectionsorder();
}
