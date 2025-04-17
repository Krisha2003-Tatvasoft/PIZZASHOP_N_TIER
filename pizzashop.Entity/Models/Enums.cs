namespace pizzashop.Entity.Models;

public class Enums
{
    public enum statustype
    {
        Active = 0,
        Inactive = 1
    }

    public enum itemtype
    {
        Veg = 0,
        NonVeg = 1,
        Vegan = 2
    }

    public enum tablestatus
    {
        Available = 0,
        Occupied = 1
    }

    public enum taxtype
    {
        Percentage =0,

        FlatAmount =1
    }

    public enum orderstatus
    {
        InProgress=0,

        Pending=1,

        Served=2,

        Completed=3,

        Cancelled=4,

        OnHold=5,

        Failed =6

    }

    public enum paymentmode{

        Online =0,

        Card=1,

        Cash =2
    }


}
