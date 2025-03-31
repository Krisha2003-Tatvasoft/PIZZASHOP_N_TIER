using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;
namespace pizzashop.Service.Implementations;

public class SectionService : ISectionService
{
   private readonly ISectionRepository _sectionRepository;

   private readonly ITableRepository _tableRepository;

   public SectionService(ISectionRepository sectionRepository, ITableRepository tableRepository)
   {
      _sectionRepository = sectionRepository;
      _tableRepository = tableRepository;
   }

   public async Task<List<VMSection>> GetSectionList()
   {
      var sectionList = await _sectionRepository.AllSections();

      var sections = sectionList.Select(c => new VMSection
      {
         Sectionid = c.Sectionid,
         Sectionname = c.Sectionname,
         Description = c.Description
      }).ToList();

      return sections;
   }

   public async Task<bool> AddSectionPost(int loginId, VMSection viewmodel)
   {
      if (await _sectionRepository.SectionNameAsync(viewmodel.Sectionname))
      {
         return false;
      }
      else
      {
         Section section = new Section
         {
            Sectionname = viewmodel.Sectionname,
            Description = viewmodel.Description,
            Createdby = loginId
         };
         await _sectionRepository.AddNewSection(section);
         return true;
      }
   }

   public async Task<VMSection> EditSection(int id)
   {
      Section section = await _sectionRepository.SecByIdAsync(id);
      VMSection model = new VMSection
      {
         Sectionid = id,
         Sectionname = section.Sectionname,
         Description = section.Description
      };
      return model;
   }

   public async Task<bool> EditSectionPost(int loginId, VMSection viewmodel)
   {
      if (await _sectionRepository.SecNameExistAtEditAsync(viewmodel.Sectionname, viewmodel.Sectionid))
      {
         return false;
      }
      else
      {
         Section section = await _sectionRepository.SecByIdAsync(viewmodel.Sectionid);

         section.Sectionname = viewmodel.Sectionname;
         section.Description = viewmodel.Description;
         section.Modifiedby = loginId;

         await _sectionRepository.UpdateSection(section);
         return true;

      }
   }

   public async Task<bool> DeleteSection(int id)
   {
      Section section = await _sectionRepository.SecByIdAsync(id);
      await _sectionRepository.DeleteSection(section);
      if (await _tableRepository.DeleteBySection(id))
      {
         return true;
      }
      else
      {
         return false;
      }
   }


   public async Task<bool> SaveOrderSection(List<int> orderIds)
   {
      var sections = await _sectionRepository.AllSectionsorder();

      if (sections == null)
      {
         return false;
      }
      foreach (var item in orderIds)
      {
         var updatesortorder = sections.Where(u => u.Sectionid == item).FirstOrDefault();

         if (updatesortorder != null)
         {
            try
            {
               updatesortorder.sortOrder = orderIds.IndexOf(item) + 1;
               await _sectionRepository.UpdateSection(updatesortorder);
            }
            catch (Exception e)
            {
               Console.WriteLine(e.Message);
               return false;
            }
         }
      }
      return true;
   }




}
