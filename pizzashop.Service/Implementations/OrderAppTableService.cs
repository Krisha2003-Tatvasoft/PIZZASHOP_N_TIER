using Newtonsoft.Json;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class OrderAppTableService : IOrderAppTableService
{
    private readonly ISectionRepository _sectionRepository;

    private readonly IWaitingTokenRepository _waitingTokenRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly IOrderRepository _orderRepository;

    private readonly IOrderTableMappingRepository _orderTableMappingRepository;

    private readonly ITableRepository _tableRepository;

    public OrderAppTableService(ISectionRepository sectionRepository, IWaitingTokenRepository waitingTokenRepository,
    ICustomerRepository customerRepository ,IOrderRepository orderRepository, IOrderTableMappingRepository orderTableMappingRepository
    ,ITableRepository tableRepository)
    {
        _sectionRepository = sectionRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _orderTableMappingRepository = orderTableMappingRepository;
        _tableRepository = tableRepository;
    }

    public async Task<AssignTable> AssignTable(int id)
    {
        AssignTable model = new AssignTable
        {
            Sectionid = id,
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }

    public async Task<int?> AssignTablePost(int loginid, AssignTable model)
    {
        if (loginid == null)
        {
            return null;
        }
        else
        {
            int? customerid = null;
            List<int> tableids = JsonConvert.DeserializeObject<List<int>>(model.TableIds);
            if (model.Waitingtokenid != null)
            {
                var waitingtoken = await _waitingTokenRepository.WTByIdAsync(model.Waitingtokenid);
                if (waitingtoken != null)
                {
                    waitingtoken.Isassigned = true;
                    waitingtoken.Sectionid = model.Sectionid;
                    waitingtoken.Noofpeople = (short)model.Noofperson;

                    await _waitingTokenRepository.UpdateWaitingToken(waitingtoken);
                }
            }
            var customer = await _customerRepository.GetCustomerByEmail(model.Email);
            if (customer != null)
            {
                Console.WriteLine("customer found");
                customer.Customername = model.Customername;
                customer.Phoneno = model.Phoneno;
                customer.Visitcount = (short?)(customer.Visitcount + 1);
                await _customerRepository.UpdateCustomer(customer);
                customerid = customer.Customerid;
            }
            else
            {
               
                customer = new Customer
                {
                    Customername = model.Customername,
                    Phoneno = model.Phoneno,
                    Email = model.Email,
                    Createdby = loginid,
                    Visitcount = 1
                };
                await _customerRepository.AddNewCustomer(customer);
                customerid = customer.Customerid;
            }
            Order order = new Order{
               Customerid = (int)customerid,
               status = (int)Entity.Models.Enums.orderstatus.Pending,
               Noofperson = (short)model.Noofperson
            };
            await _orderRepository.AddNewOrder(order);
            foreach(var table in tableids)
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

            return order.Orderid;
        }
    }

}
