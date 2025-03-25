using pizzashop.Entity.Models;
namespace pizzashop.Repository.Interfaces;

public interface IItemmodifiergroupmapRepository
{
    Task AddNewMapping(Itemmodifiergroupmap itemmodifiergroupmap);

    Task<List<Itemmodifiergroupmap>> GetMGMByitemid(int id);

    Task UpdateMapping(Itemmodifiergroupmap mapping);

    Task DeleteMapping(Itemmodifiergroupmap mapping);

    Task DeleteMappingByItem(int id);

    
}
