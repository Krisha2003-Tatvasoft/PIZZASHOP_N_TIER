using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IKOTService
{
    Task<(List<Ticket>, int totalOrder)> Ticket(int id, string status, int page);
    Task<Ticket> TicketDetails(int id, string status);
}
