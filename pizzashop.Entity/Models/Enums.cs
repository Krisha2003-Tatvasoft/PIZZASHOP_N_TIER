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
        Occupied = 1,
        Reserved = 2
    }

    public enum taxtype
    {
        Percentage =0,

        FlatAmount =1
    }


}
