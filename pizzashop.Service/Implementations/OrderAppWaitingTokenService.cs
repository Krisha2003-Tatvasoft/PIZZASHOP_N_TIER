using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
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

    private readonly IOrderRepository _orderRepository;

    private readonly IOrderTableMappingRepository _orderTableMappingRepository;

    private readonly ITableRepository _tableRepository;



    public OrderAppWaitingTokenService(ISectionRepository sectionRepository, ICustomerRepository customerRepository
    , IWaitingTokenRepository waitingTokenRepository, IOrderRepository orderRepository, IOrderTableMappingRepository orderTableMappingRepository
    , ITableRepository tableRepository)
    {
        _sectionRepository = sectionRepository;
        _customerRepository = customerRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _orderRepository = orderRepository;
        _orderTableMappingRepository = orderTableMappingRepository;
        _tableRepository = tableRepository;
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

    public async Task<(bool Success, string Message)> AddWaitingTokenPost(int loginId, AddWaitingToken model)
    {
        if (model == null)
        {
            return (false, "no data given");
        }
        var customer = await _customerRepository.GetCustomerByEmail(model.Email);

        if (customer != null)
        {
            var waitingList = await _waitingTokenRepository.GetWaitingList();

            foreach (var waiting in waitingList)
            {
                if (waiting.Customerid == customer.Customerid &&
                waiting.Createdat.HasValue && waiting.Createdat.Value.Date == DateTime.UtcNow.Date)
                {
                    return (false, "This Customer is Already Present in WaitingToken.");
                }
            }


            var orders = customer.Orders?.ToList(); // Ensure it's not null


            if (orders != null && orders.Any())
            {
                var latestOrder = orders.OrderByDescending(o => o.Orderid).FirstOrDefault();

                if (latestOrder != null && (latestOrder.status == 0 || latestOrder.status == 1 || latestOrder.status == 2))
                {
                    return (false, "This customer already has an active or recent table assignment.");
                }
            }

            customer.Customername = model.Customername;
            customer.Phoneno = model.Phoneno;
            customer.Visitcount = (short?)(customer.Visitcount + 1);
            customer.Modifiedat = DateTime.Now;
            customer.Modifiedby = loginId;
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
                Visitcount = 1,
                Totalorder = 0,
                Createdby = loginId,
                Createdat = DateTime.Now,
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
        return (true, "WaitingToken Created Sucessfully.");
    }

    public async Task<List<WaitingListTable>> WaitingListBySectionId(List<int> sectionId)
    {
        var waiting = new List<Waitingtoken>();
        if (sectionId == null || sectionId.Count == 0)
        {
            waiting = await _waitingTokenRepository.GetWaitingList();
        }
        else
        {
            foreach (var id in sectionId)
            {
                var waitingForSec = await _waitingTokenRepository.WaitingListBySectionId(id);
                waiting.AddRange(waitingForSec);
            }
        }
        if (waiting == null || waiting.Count == 0)
        {
            return new List<WaitingListTable>();
        }

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
            Sectionid = new List<int> { wt.Sectionid },
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
            Customername = w.Customer.Customername,
            Noofperson = (short)w.Noofpeople,
            Phoneno = w.Customer.Phoneno,
            Email = w.Customer.Email,
            Sectionid = w.Sectionid
        }
        ).ToList();

        return waitingList;
    }

    public async Task<AddWaitingToken> EditGetWT(int id)
    {
        Waitingtoken waitingtoken = await _waitingTokenRepository.WTByIdAsync(id);
        AddWaitingToken model = new AddWaitingToken
        {
            Email = waitingtoken.Customer.Email,
            Customername = waitingtoken.Customer.Customername,
            Phoneno = waitingtoken.Customer.Phoneno,
            Noofperson = (short)waitingtoken.Noofpeople,
            Sectionid = waitingtoken.Sectionid,
            Waitingtokenid = waitingtoken.Waitingtokenid,
            customerId = waitingtoken.Customer.Customerid,
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }


    public async Task<bool> EditPosttWT(int loginId, AddWaitingToken model)
    {
        Customer oldcustomer = null;
        if (model == null)
        {
            return false;
        }

        Waitingtoken waitingtoken = await _waitingTokenRepository.WTByIdAsync(model.Waitingtokenid);

        waitingtoken.Noofpeople = (short)model.Noofperson;
        waitingtoken.Sectionid = model.Sectionid;
        waitingtoken.Modifiedby = loginId;
        var existcustomer = await _customerRepository.GetCustomerByEmail(model.Email);
        if (existcustomer != null && existcustomer.Customerid != waitingtoken.Customer.Customerid)
        {
            oldcustomer = await _customerRepository.GetCustomerByEmail(waitingtoken.Customer.Email);
            waitingtoken.Customerid = existcustomer.Customerid;
        }

        await _waitingTokenRepository.UpdateWaitingToken(waitingtoken);

        Customer customer = await _customerRepository.GetCustomerById(waitingtoken.Customerid);
        customer.Email = model.Email;
        customer.Customername = model.Customername;
        customer.Phoneno = model.Phoneno;
        customer.Modifiedby = loginId;

        await _customerRepository.UpdateCustomer(customer);
        if (oldcustomer != null && oldcustomer.Visitcount == 0)
        {
            await _customerRepository.Delete(oldcustomer);
        }

        return true;
    }



    public async Task<bool> DeleteWT(int id)
    {
        Waitingtoken waitingtoken = await _waitingTokenRepository.WTByIdAsync(id);
        if (waitingtoken == null)
        {
            return false;
        }
        else
        {
            await _waitingTokenRepository.Delete(waitingtoken);
            return true;
        }

    }

    public async Task<(int? orderid, string Message)> AssignTablePost(int loginid, WaitingListAssignTable model)
    {

        if (loginid == null)
        {
            return (null, "loginId is null.");
        }
        else
        {
            List<int> tableids = JsonConvert.DeserializeObject<List<int>>(model.TableIds);
            var waitingtoken = await _waitingTokenRepository.WTByIdAsync(model.Waitingtokenid);
            var totalCapacity = 0;
            foreach (var id in tableids)
            {
                var table = await _tableRepository.TableByIdAsync(id);
                totalCapacity += (int)table.Capacity;
            }

            if (totalCapacity < waitingtoken.Noofpeople)
            {
                return (null, "No. of Person Is more Then the Table Capacity.");
            }

            if (waitingtoken != null)
            {
                waitingtoken.Isassigned = true;
                waitingtoken.Modifiedat = DateTime.Now;
                waitingtoken.Modifiedby = loginid;
                await _waitingTokenRepository.UpdateWaitingToken(waitingtoken);
            }

            var customer = await _customerRepository.GetCustomerByEmail(waitingtoken?.Customer.Email);
            if (customer != null)
            {
                var orders = customer.Orders?.ToList(); // Ensure it's not null
                if (orders != null && orders.Any())
                {
                    var latestOrder = orders.OrderByDescending(o => o.Orderid).FirstOrDefault();

                    if (latestOrder != null && (latestOrder.status == 0 || latestOrder.status == 1 || latestOrder.status == 2))
                    {
                        return (null, "This customer already has an active or recent table assignment.");
                    }
                }
                customer.Totalorder++;
                customer.Modifiedby = loginid;
                customer.Modifiedat = DateTime.Now;
                await _customerRepository.UpdateCustomer(customer);
            }

            Order order = new Order
            {
                Customerid = customer.Customerid,
                status = (int)Entity.Models.Enums.orderstatus.Pending,
                Noofperson = (short)waitingtoken.Noofpeople
            };
            await _orderRepository.AddNewOrder(order);
            foreach (var table in tableids)
            {
                Ordertable ordertable = new Ordertable
                {
                    Orderid = order.Orderid,
                    Tableid = table
                };
                var tabledata = await _tableRepository.TableByIdAsync(table);
                tabledata.tablestatus = Enums.tablestatus.Occupied;
                await _tableRepository.UpdateTable(tabledata);
                await _orderTableMappingRepository.AddNewOrderMapping(ordertable);
            }
            return (order.Orderid, "Table Assign Sucessfully.");

        }
    }




}
