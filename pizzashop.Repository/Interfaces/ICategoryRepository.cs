using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task addAsync(Category category);

    List<Category> AllCategory();
}
