namespace pizzashop.Entity.ViewModels;

public class WaitingListTable
{
    public string Customername { get; set; } = null!;

    public short? Noofperson { get; set; }

    public int Waitingtokenid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? WaitingTime { get; set; }

    public string Phoneno { get; set; } = null!;

    public string? Email { get; set; }



}
