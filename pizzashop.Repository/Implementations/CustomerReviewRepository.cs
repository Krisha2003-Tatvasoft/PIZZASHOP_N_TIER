using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class CustomerReviewRepository : ICustomerReviewRepository
{
    private readonly PizzashopContext _context;

    public CustomerReviewRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddNewReview(Customerreview review)
    {
        _context.Customerreviews.Add(review);
        await _context.SaveChangesAsync();
    }

}
