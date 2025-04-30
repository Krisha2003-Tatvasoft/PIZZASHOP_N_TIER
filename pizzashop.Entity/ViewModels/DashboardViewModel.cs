namespace pizzashop.Entity.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AvgOrder { get; set; }
    public string AvgWaitingTime { get; set; }
    public int WaitingCount { get; set; }
    public int NewCustomers { get; set; }

    public List<ItemStats> TopSellingItems { get; set; }
    public List<ItemStats> LeastSellingItems { get; set; }

    public List<ChartPoint> RevenueGrowthData { get; set; }
    public List<ChartPoint> CustomerGrowthData { get; set; }
}

