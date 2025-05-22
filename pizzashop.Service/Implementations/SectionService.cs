using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;
using VMSection = pizzashop.Entity.ViewModels.Section;
using VMTable = pizzashop.Entity.ViewModels.Table;
using VMOrderTableView = pizzashop.Entity.ViewModels.OrderTableView;
using VMSectionWIthCount = pizzashop.Entity.ViewModels.SectionWithCount;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace pizzashop.Service.Implementations;

public class SectionService : ISectionService
{
   private readonly ISectionRepository _sectionRepository;

   private readonly ITableRepository _tableRepository;

   private readonly IWaitingTokenRepository _waitingTokenRepository;


   public SectionService(ISectionRepository sectionRepository, ITableRepository tableRepository, IWaitingTokenRepository waitingTokenRepository)
   {
      _sectionRepository = sectionRepository;
      _tableRepository = tableRepository;
      _waitingTokenRepository = waitingTokenRepository;
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

   public async Task<(bool sucess,string message)> DeleteSection(int id)
   {
      Section section = await _sectionRepository.SecByIdAsync(id);
      List<Table> tables = await _tableRepository.tablesBysection(id);
      foreach (var table in tables)
      {
         if (table.tablestatus == Enums.tablestatus.Occupied)
         {
            return (false, "This section has 1 or more occupied Table You can't delete this setion.");
         }
      }
      await _sectionRepository.DeleteSection(section);
      if (await _tableRepository.DeleteBySection(id))
      {
         return (true, "Section Deleted Suceesfully.");
      }
      else
      {
         return (false, "error in delete Section.");
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

   private static Dictionary<int, DateTime> _orderStartTimes = new();

   public async Task<List<VMOrderTableView>> OrderTableViews()
   {
      List<Section> sections = await _sectionRepository.GetSectionWithTables();
      sections = sections.Where(s => s.Tables != null && s.Tables.Any()).ToList();

      var orderTableViews = sections.Select(s => new VMOrderTableView
      {
         Sectionid = s.Sectionid,
         Sectionname = s.Sectionname,
         Tables = s.Tables.Select(static t =>
         {
            var orderTable = t.Ordertables?
           .OrderByDescending(ot => ot.Order.Orderid)
           .FirstOrDefault();
            var order = orderTable?.Order;
            var status = order != null ? (Enums.orderstatus)order.status : Enums.orderstatus.Pending;
            var Orderid = order?.Orderid;

            string? runningSince = null;

            if (status == Enums.orderstatus.InProgress || status == Enums.orderstatus.Served || status == Enums.orderstatus.Pending)
            {
               if (!_orderStartTimes.ContainsKey(t.Tableid))
               {
                  _orderStartTimes[t.Tableid] = order?.Orderdate ?? DateTime.Now;
               }

               var duration = DateTime.Now - _orderStartTimes[t.Tableid];
               runningSince = $"{(duration.Days > 0 ? duration.Days + " days " : "")}" +
                  $"{(duration.Hours > 0 ? duration.Hours + " hours\n" : "")}" +
                  $"{duration.Minutes} min {duration.Seconds} sec";
            }
            else
            {
               if (_orderStartTimes.ContainsKey(t.Tableid))
               {
                  _orderStartTimes.Remove(t.Tableid);
               }
            }

            return new VMTable
            {
               Tableid = t.Tableid,
               Tablename = t.Tablename,
               Capacity = t.Capacity,
               tablestatus = t.tablestatus,
               orderstatus = status,
               Totalamount = order?.Totalamount ?? 0,
               RunningSince = runningSince,
               Orderid = Orderid
            };
         }).ToList()
      }).ToList();

      return orderTableViews;
   }


   public async Task<List<VMSectionWIthCount>> GetSectionWithCount()
   {
      // var sectionList = await _sectionRepository.AllSections();
      // var alltokens = await _waitingTokenRepository.GetWaitingList();

      // var sections = sectionList.Select(c => new VMSectionWIthCount
      // {
      //    Sectionid = c.Sectionid,
      //    Sectionname = c.Sectionname,
      //    TokenCount = alltokens.Where(w=> w.Createdat != null && w.Createdat.Value.Date == DateTime.Today).Count(t => t.Sectionid == c.Sectionid)

      // }).ToList();

      // var sectionWithCountList = sections.ToList();
      // sectionWithCountList.Insert(0, new VMSectionWIthCount
      // {
      //    Sectionid = 0,
      //    Sectionname = "All",
      //    TokenCount = alltokens.Where(w=> w.Createdat != null && w.Createdat.Value.Date == DateTime.Today).Count()
      // });
      // sections = sectionWithCountList;

      // return sections;
      return await _sectionRepository.GetWTSectionListFromSP();
   }
   

   public async Task<List<SelectListItem>> GetSectionListDD()
   {
      return  await _sectionRepository.SectionDDAvailable();
   }





}
