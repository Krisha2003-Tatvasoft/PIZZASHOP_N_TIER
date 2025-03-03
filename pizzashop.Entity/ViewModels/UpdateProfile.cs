namespace pizzashop.Entity.ViewModels;

public class UpdateProfile
{
    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    // public string? Profileimg { get; set; }

    public string Phone { get; set; } = null!;

    public int Countryid { get; set; }

    public int Stateid { get; set; }

    public int Cityid { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

}
