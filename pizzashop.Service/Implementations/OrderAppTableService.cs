using DocumentFormat.OpenXml.Spreadsheet;
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
    ICustomerRepository customerRepository, IOrderRepository orderRepository, IOrderTableMappingRepository orderTableMappingRepository
    , ITableRepository tableRepository)
    {
        _sectionRepository = sectionRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _orderTableMappingRepository = orderTableMappingRepository;
        _tableRepository = tableRepository;
    }

    public async Task<AssignTable> AssignTable(List<int> id)
    {
        AssignTable model = new AssignTable
        {
            Sectionid = id,
            SectionList = await _sectionRepository.SectionDDAsync()
        };
        return model;
    }

    public async Task<(int? orderid, string Message)> AssignTablePost(int loginid, AssignTable model)
    {
        // if (loginid == null)
        // {
        //     return (null, "loginId is null.");
        // }
        // else
        // {
        //     int? customerid = null;
        //     List<int> tableids = JsonConvert.DeserializeObject<List<int>>(model.TableIds);

        //     var totalCapacity = 0;
        //     foreach (var id in tableids)
        //     {
        //         var table = await _tableRepository.TableByIdAsync(id);
        //         totalCapacity += (int)table.Capacity;
        //     }
        //     if (totalCapacity < model.Noofperson)
        //     {
        //         return (null, "No. of Person " + model.Noofperson + " Is more Then the Table Capacity " + totalCapacity + ".");
        //     }

        //     if (model.Waitingtokenid != null)
        //     {
        //         var waitingtoken = await _waitingTokenRepository.WTByIdAsync(model.Waitingtokenid);
        //         if (waitingtoken != null)
        //         {
        //             waitingtoken.Isassigned = true;
        //             waitingtoken.Noofpeople = (short)model.Noofperson;
        //             waitingtoken.Modifiedat = DateTime.Now;;
        //             waitingtoken.Modifiedby = loginid;
        //             await _waitingTokenRepository.UpdateWaitingToken(waitingtoken);
        //         }
        //         var customer = await _customerRepository.GetCustomerByEmail(waitingtoken.Customer.Email);
        //         if (customer != null)
        //         {
        //             var orders = customer.Orders?.ToList(); // Ensure it's not null
        //             if (orders != null && orders.Any())
        //             {
        //                 var latestOrder = orders.OrderByDescending(o => o.Orderid).FirstOrDefault();

        //                 if (latestOrder != null && (latestOrder.status == 0 || latestOrder.status == 1 || latestOrder.status == 2))
        //                 {
        //                     return (null, "This customer already has an active or recent table assignment.");
        //                 }
        //             }
        //             // customer.Visitcount = (short?)(customer.Visitcount + 1);
        //             customer.Totalorder++;
        //             customer.Modifiedby = loginid;
        //             customer.Modifiedat = DateTime.Now;
        //             await _customerRepository.UpdateCustomer(customer);
        //             customerid = customer.Customerid;
        //         }
        //     }
        //     else
        //     {
        //         var customer = await _customerRepository.GetCustomerByEmail(model.Email);
        //         if (customer != null)
        //         {
        //             var orders = customer.Orders?.ToList(); // Ensure it's not null
        //             if (orders != null && orders.Any())
        //             {
        //                 var latestOrder = orders.OrderByDescending(o => o.Orderid).FirstOrDefault();

        //                 if (latestOrder != null && (latestOrder.status == 0 || latestOrder.status == 1 || latestOrder.status == 2))
        //                 {
        //                     return (null, "This customer already has an active or recent table assignment.");
        //                 }
        //             }

        //             customer.Customername = model.Customername;
        //             customer.Phoneno = model.Phoneno;
        //             customer.Visitcount = (short?)(customer.Visitcount + 1);
        //             customer.Totalorder++;
        //             customer.Modifiedby = loginid;
        //             customer.Modifiedat = DateTime.Now;

        //             if (model.Waitingtokenid == null)
        //             {
        //                 var waitingList = await _waitingTokenRepository.GetWaitingList();

        //                 foreach (var waiting in waitingList)
        //                 {
        //                     if (waiting.Customerid == customer.Customerid &&
        //                     waiting.Createdat.HasValue && waiting.Createdat.Value.Date == DateTime.UtcNow.Date)
        //                     {
        //                         return (null, "This Customer is Already Present in WaitingToken Assign From there.");
        //                     }
        //                 }
        //             }

        //             await _customerRepository.UpdateCustomer(customer);
        //             customerid = customer.Customerid;
        //         }
        //         else
        //         {
        //             customer = new Customer
        //             {
        //                 Customername = model.Customername,
        //                 Phoneno = model.Phoneno,
        //                 Email = model.Email,
        //                 Createdby = loginid,
        //                 Visitcount = 1,
        //                 Totalorder = 1,
        //                 Createdat = DateTime.Now
        //             };
        //             await _customerRepository.AddNewCustomer(customer);
        //             customerid = customer.Customerid;
        //         }

        //     }

        //     Order order = new Order
        //     {
        //         Customerid = (int)customerid,
        //         status = (int)Entity.Models.Enums.orderstatus.Pending,
        //         Noofperson = (short)model.Noofperson
        //     };
        //     await _orderRepository.AddNewOrder(order);

        //     foreach (var table in tableids)
        //     {
        //         Ordertable ordertable = new Ordertable
        //         {
        //             Orderid = order.Orderid,
        //             Tableid = table
        //         };
        //         var tabledata = await _tableRepository.TableByIdAsync(table);
        //         tabledata.tablestatus = Enums.tablestatus.Occupied;
        //         await _tableRepository.UpdateTable(tabledata);
        //         await _orderTableMappingRepository.AddNewOrderMapping(ordertable);
        //     }



        //     return (order.Orderid, "Table Assign Sucessfully.");
        // }

        List<int> tableids = JsonConvert.DeserializeObject<List<int>>(model.TableIds);

        return await _waitingTokenRepository.Assign_From_Tables_SP(loginid, model, tableids);
    }

}
