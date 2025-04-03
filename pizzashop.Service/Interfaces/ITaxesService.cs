using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface ITaxesService
{
    Task<(List<TaxTable>, int totalTaxes)> GetTaxTable(int page, int pageSize, string search);

    Task<bool> AddTaxPost(int loginId, AddTax viewmodel);

    Task<AddTax> EditTax(int id);

    Task<bool> EditTaxPost(int loginId, AddTax viewmodel);

    Task<bool> DeleteTax(int id);

     Task<bool> UpdateEnable(int loginId, int id ,bool enable);
    
}
