using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ITaxesRepository
{
 Task<List<Taxis>> GetTaxTableAsync(string search);

 Task<bool> TaxesNameExist(string taxname);

 Task AddNewTax(Taxis tax);

 Task<bool> TaxNameExistInEdit(string taxname, int taxid);

 Task UpdateTax(Taxis tax);

 Task<Taxis> TaxByIdAsync(int id);

 Task DeleteTax(Taxis tax);

}
