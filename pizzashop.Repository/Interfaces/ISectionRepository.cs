using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ISectionRepository
{
    Task<List<Section>> AllSections();
}
