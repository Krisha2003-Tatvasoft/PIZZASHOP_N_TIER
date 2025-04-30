using pizzashop.Entity.ViewModels;

namespace pizzashop.Service.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(DateTime? fromDate, DateTime? toDate);
}
