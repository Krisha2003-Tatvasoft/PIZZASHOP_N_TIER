using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;


public class KOTService : IKOTService
{
    private readonly IOrderRepository _orderRepository;
    public KOTService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }


    private static Dictionary<int, DateTime> _orderStartTimes = new();
    public async Task<(List<Ticket>, int totalOrder)> Ticket(int id, string status)
    {
        var orders = await _orderRepository.InprogressOrders();
        int totalOrder = 0;
        List<Ticket> tickets = new List<Ticket>();

        foreach (var order in orders)
        {
            var orderdeatils = await _orderRepository.OrderDetailsByIdAsync(order.Orderid);
            List<Ordereditem> ItemsByFilter = new List<Ordereditem>();
            if (status == "Inprogress")
            {
                ItemsByFilter = orderdeatils.Ordereditems.Where(i => (i.Quantity - i.ReadyQuantity) > 0 && (id != 0 ? i.Item.Categoryid == id : true)).ToList();
            }
            else
            {
                ItemsByFilter = orderdeatils.Ordereditems.Where(i => i.ReadyQuantity > 0 && (id != 0 ? i.Item.Categoryid == id : true)).ToList();
            }

            if (ItemsByFilter.Any())
            {
                List<OrderItem> items = ItemsByFilter
                .Select(i => new OrderItem
                {
                    Itemid = i.Itemid,
                    Itemname = i.Item.Itemname,
                    Quantity = (short)(status == "Inprogress" ? i.Quantity - i.ReadyQuantity : i.ReadyQuantity),
                    Itemwisecomment = i.Itemwisecomment,
                    Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
                    {
                        Modifierid = m.Modifiers.Modifierid,
                        Modifiername = m.Modifiers.Modifiername,
                        Quantity = (short)i.Quantity
                    }).ToList()
                }).ToList();

                var orderstatus = order != null ? (Enums.orderstatus)order.status : Enums.orderstatus.Pending;
                string? runningSince = null;
                if (orderstatus == Enums.orderstatus.InProgress || orderstatus == Enums.orderstatus.Served || orderstatus == Enums.orderstatus.Pending)
                {
                    if (!_orderStartTimes.ContainsKey(order.Orderid))
                    {
                        _orderStartTimes[order.Orderid] = order?.Orderdate ?? DateTime.Now;
                    }

                    var duration = DateTime.Now - _orderStartTimes[order.Orderid];
                    runningSince = $"{(duration.Days > 0 ? duration.Days + " days " : "")}" +
                       $"{(duration.Hours > 0 ? duration.Hours + " hours\n" : "")}" +
                       $"{duration.Minutes} min {duration.Seconds} sec";
                }
                else
                {
                    if (_orderStartTimes.ContainsKey(order.Orderid))
                    {
                        _orderStartTimes.Remove(order.Orderid);
                    }
                }


                Ticket model = new Ticket
                {
                    Orderid = order.Orderid,
                    Tablenames = order.Ordertables.Select(t => t.Table.Tablename).ToList(),
                    Sectionname = order.Ordertables.Select(t => t.Table.Section.Sectionname).Distinct().ToList(),
                    Orderwisecomment = order.Orderwisecomment,
                    Items = items,
                    RunningSince = runningSince,
                    orderstatus = (Enums.orderstatus)order.status
                };
                tickets.Add(model);

            }
        }
        totalOrder = tickets.Count;
        tickets = tickets.ToList();
        return (tickets, totalOrder); // Return the list of tickets

    }


    public async Task<Ticket> TicketDetails(int id, string status)
    {

        var orderdeatils = await _orderRepository.OrderDetailsByIdAsync(id);
        List<Ordereditem> ItemsByFilter = new List<Ordereditem>();
        if (status == "Inprogress")
        {
            ItemsByFilter = orderdeatils.Ordereditems.Where(i => (i.Quantity - i.ReadyQuantity) > 0).ToList();
        }
        else
        {
            ItemsByFilter = orderdeatils.Ordereditems.Where(i => i.ReadyQuantity > 0).ToList();
        }

        List<OrderItem> items = ItemsByFilter
        .Select(i => new OrderItem
        {
            Itemid = i.Itemid,
            Orderitemid = i.Ordereditemid,
            Itemname = i.Item.Itemname,
            Quantity = (short)(status == "Inprogress" ? i.Quantity - i.ReadyQuantity : i.ReadyQuantity),
            Modifiers = i.Ordereditemmodifers.Select(m => new OrderModifier
            {
                Modifierid = m.Modifiers.Modifierid,
                Modifiername = m.Modifiers.Modifiername,
                Quantity = (short)i.Quantity
            }).ToList()
        }).ToList();

        Ticket model = new Ticket
        {
            Orderid = id,
            Items = items
        };


        return model; // Return the list of tickets

    }

    public async Task UpdateItemStatusAsync(OrderItemStatus model)
    {
        await _orderRepository.UpdateItemStatusAsync(model);
    }




}
