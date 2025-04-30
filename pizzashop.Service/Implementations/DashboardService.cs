using pizzashop.Entity.ViewModels;
using pizzashop.Repository.Interfaces;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;

    public DashboardService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime? fromDate, DateTime? toDate)
    {
        return await _orderRepository.GetDashboardData(fromDate, toDate);
    }


}
