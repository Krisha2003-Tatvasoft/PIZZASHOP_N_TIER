using DocumentFormat.OpenXml.Wordprocessing;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class OrderAppWaitingTokenService : IOrderAppWaitingTokenService
{
    private readonly ISectionRepository _sectionRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly IWaitingTokenRepository _waitingTokenRepository;

    public OrderAppWaitingTokenService(ISectionRepository sectionRepository, ICustomerRepository customerRepository
    , IWaitingTokenRepository waitingTokenRepository)
    {
        _sectionRepository = sectionRepository;
        _customerRepository = customerRepository;
        _waitingTokenRepository = waitingTokenRepository;
    }

    public async Task<AddWaitingToken> AddWaitingToken(int id)
    {
        AddWaitingToken model = new AddWaitingToken
        {
            Sectionid = id,
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }

    public async Task<bool> AddWaitingTokenPost(int loginId, AddWaitingToken model)
    {
        if (model == null)
        {
            return false;
        }
        var customer = await _customerRepository.GetCustomerByEmail(model.Email);
        if (customer != null)
        {
            customer.Customername = model.Customername;
            customer.Phoneno = model.Phoneno;
            await _customerRepository.UpdateCustomer(customer);

            Waitingtoken token = new Waitingtoken
            {
                Customerid = customer.Customerid,
                Noofpeople = (short)model.Noofperson,
                Sectionid = model.Sectionid,
                Isassigned = false,
                Createdby = loginId
            };
            await _waitingTokenRepository.AddNewWaitingToken(token);
        }
        else
        {
            customer = new Customer
            {
                Customername = model.Customername,
                Phoneno = model.Phoneno,
                Email = model.Email,
                Createdby = loginId
            };
            await _customerRepository.AddNewCustomer(customer);
            Waitingtoken token = new Waitingtoken
            {
                Customerid = customer.Customerid,
                Noofpeople = (short)model.Noofperson,
                Sectionid = model.Sectionid,
                Isassigned = false,
                Createdby = loginId
            };
            await _waitingTokenRepository.AddNewWaitingToken(token);
        }
        return true;
    }
}
