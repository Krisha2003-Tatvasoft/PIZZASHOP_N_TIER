using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class TaxesService : ITaxesService
{

    private readonly ITaxesRepository _taxesRepository;

    public TaxesService(ITaxesRepository taxesRepository)
    {
        _taxesRepository = taxesRepository;
    }


    public async Task<(List<TaxTable>, int totalTaxes)> GetTaxTable(int page, int pageSize, string search)
    {
        var taxList = await _taxesRepository.GetTaxTableAsync(search);
        int totalTaxes = taxList.Count();

        var taxes =  taxList
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new TaxTable
        {
           Taxid = u.Taxid,
           Taxname = u.Taxname,
           taxtype= (Entity.Models.Enums.taxtype)u.Taxtype,
           Isenabled=(bool)u.Isenabled,
           Isdefault = (bool)u.Isdefault,
           Taxvalue=u.Taxvalue
        })
        .ToList();

        return (taxes, totalTaxes);
    }
}
