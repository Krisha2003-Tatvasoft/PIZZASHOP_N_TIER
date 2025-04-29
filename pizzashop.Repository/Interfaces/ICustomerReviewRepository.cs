using pizzashop.Entity.Models;

namespace pizzashop.Repository.Interfaces;

public interface ICustomerReviewRepository
{
    Task AddNewReview(Customerreview review);
}
