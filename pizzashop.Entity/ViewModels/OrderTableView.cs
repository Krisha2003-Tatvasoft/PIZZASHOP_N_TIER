namespace pizzashop.Entity.ViewModels;

public class OrderTableView
{
      public int Sectionid { get; set; }
      public string Sectionname { get; set; } = null!;
      public List<Table> Tables { get; set; } = new List<Table>(); 

}
