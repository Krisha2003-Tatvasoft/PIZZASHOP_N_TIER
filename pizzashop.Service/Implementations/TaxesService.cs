using pizzashop.Entity.Models;
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

        var taxes = taxList
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new TaxTable
        {
            Taxid = u.Taxid,
            Taxname = u.Taxname,
            taxtype = (Entity.Models.Enums.taxtype)u.Taxtype,
            Isenabled = (bool)u.Isenabled,
            Isdefault = (bool)u.Isdefault,
            Taxvalue = u.Taxvalue
        })
        .ToList();

        return (taxes, totalTaxes);
    }


    public async Task<bool> AddTaxPost(int loginId, AddTax viewmodel)
    {
        if (await _taxesRepository.TaxesNameExist(viewmodel.Taxname))
        {
            return false;
        }
        else
        {
            Taxis tax = new Taxis
            {
                Taxname = viewmodel.Taxname,
                Taxtype = (int)viewmodel.taxtype,
                Taxvalue = viewmodel.Taxvalue,
                Isdefault = viewmodel.Isdefault,
                Isenabled = viewmodel.Isenabled,
                Createdby = loginId
            };
            await _taxesRepository.AddNewTax(tax);
            return true;
        }
    }

    public async Task<AddTax> EditTax(int id)
    {
        Taxis tax = await _taxesRepository.TaxByIdAsync(id);
        AddTax model = new AddTax
        {
            Taxname = tax.Taxname,
            taxtype = (Enums.taxtype)tax.Taxtype,
            Isenabled = (bool)tax.Isenabled,
            Isdefault = (bool)tax.Isdefault,
            Taxvalue = tax.Taxvalue,
            Taxid = id
        };
        return model;
    }

     public async Task<bool> EditTaxPost(int loginId, AddTax viewmodel)
    {
        if (await _taxesRepository.TaxNameExistInEdit(viewmodel.Taxname,viewmodel.Taxid))
        {
            return false;
        }
        else
        {
         Taxis tax = await _taxesRepository.TaxByIdAsync(viewmodel.Taxid);

            tax.Taxname=viewmodel.Taxname;
            tax.Taxtype=(int)viewmodel.taxtype;
            tax.Isdefault=viewmodel.Isdefault;
            tax.Isenabled=viewmodel.Isenabled;
            tax.Taxvalue=viewmodel.Taxvalue;
            tax.Modifiedby = loginId;

            await _taxesRepository.UpdateTax(tax);
            return true;
        }
    }

     public async Task<bool> DeleteTax(int id)
    {
        Taxis tax = await _taxesRepository.TaxByIdAsync(id);
        if (tax == null)
        {
            return false;
        }
        else
        {
            await _taxesRepository.DeleteTax(tax);
            return true;
        }

    }


}
