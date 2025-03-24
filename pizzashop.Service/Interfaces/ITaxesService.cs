using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface ITaxesService
{
    Task<(List<TaxTable>, int totalTaxes)> GetTaxTable(int page, int pageSize, string search);
}
