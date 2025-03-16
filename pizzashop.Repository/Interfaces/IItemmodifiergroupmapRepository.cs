using pizzashop.Entity.Models;
namespace pizzashop.Repository.Interfaces;

public interface IItemmodifiergroupmapRepository
{
    Task AddNewMapping(Itemmodifiergroupmap itemmodifiergroupmap);
}
