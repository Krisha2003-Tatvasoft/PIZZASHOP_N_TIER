using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IKOTService
{
    Task<List<Ticket>> Ticket(int id, string status);
    Task<Ticket> TicketDetails(int id, string status,int categoryId);

    Task UpdateItemStatusAsync(OrderItemStatus model);

}
