using Customer.Api.Entities;

namespace Customer.Api.Repositories
{
    public interface ICustomerRepository
    {
        Task<CustomerEntity> Get(int id);
        Task Add(CustomerEntity item);
        Task Delete(CustomerEntity item);
        Task<CustomerEntity> Update(CustomerEntity item);

        Task<IEnumerable<CustomerEntity>> List(string? name, int offset, int limit, string? sortBy);

        Task<int> Count(string name);

        Task Save();
    }
}