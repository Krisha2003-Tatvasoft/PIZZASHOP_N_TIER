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

    public async Task<List<WaitingListTable>> WaitingListBySectionId(int sectionId)
    {
        var waiting = await _waitingTokenRepository.WaitingListBySectionId(sectionId);

        var waitingList = waiting
        .Select(w => new WaitingListTable
        {
            Waitingtokenid = w.Waitingtokenid,
            Customername = w.Customer.Customername,
            Noofperson = (short)w.Noofpeople
        }
        ).ToList();

        return waitingList;
    }

    public async Task<AssignTable> DetailsFromWT(int id)
    {
        var wt = await _waitingTokenRepository.WTByIdAsync(id);
        var AssignTable = new AssignTable
        {
            Customername = wt.Customer.Customername,
            Email = wt.Customer.Email,
            Phoneno = wt.Customer.Phoneno,
            Noofperson = (short)wt.Noofpeople,
            Sectionid = wt.Sectionid,
            Waitingtokenid = wt.Waitingtokenid,
        };
        return AssignTable;

    }

    public async Task<List<WaitingListTable>> WaitingList(int sectionId)
    {
        List<Waitingtoken> waiting = null;

        if (sectionId == 0)
        {
          waiting = await _waitingTokenRepository.GetWaitingList();
        }
        else
        {
            waiting = await _waitingTokenRepository.WaitingListBySectionId(sectionId);
        }

        var waitingList = waiting
        .Select(static w => new WaitingListTable
        {
            Waitingtokenid = w.Waitingtokenid,
            Createdat = w.Createdat,
            WaitingTime =DateTime.Now,
            Customername = w.Customer.Customername,
            Noofperson = (short)w.Noofpeople,
            Phoneno = w.Customer.Phoneno,
            Email = w.Customer.Email
        }
        ).ToList();

        return waitingList;
    }




}
