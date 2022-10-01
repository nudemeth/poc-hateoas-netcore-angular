using System.Linq.Dynamic.Core;
using Customer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext dbContext;

        public CustomerRepository(CustomerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Add(CustomerEntity item)
        {
            await dbContext.Customers.AddAsync(item);
        }

        public async Task<int> Count(string name)
        {
            return await dbContext.Customers.CountAsync(c => string.IsNullOrWhiteSpace(name) || c.Name.Contains(name));
        }

        public Task Delete(CustomerEntity customer)
        {
            return Task.FromResult(dbContext.Customers.Remove(customer));
        }

        public async Task<CustomerEntity> Get(int id)
        {
            return await dbContext.Customers.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CustomerEntity>> List(string? name, int offset, int limit, string? sortBy)
        {
            var orderDirection = "asc";
            var orderBy = "createdAt";

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                orderBy = sortBy.Replace("-", "").Replace("+", "");
                orderDirection = sortBy.StartsWith("-") ? "desc" : "asc";
            }

            var customers = await dbContext.Customers
                .Where(c => string.IsNullOrWhiteSpace(name) || c.Name.Contains(name))
                .OrderBy($"{orderBy} {orderDirection}")
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return customers;
        }

        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }

        public Task<CustomerEntity> Update(CustomerEntity item)
        {
            dbContext.Customers.Update(item);
            return Task.FromResult(item);
        }
    }
}
