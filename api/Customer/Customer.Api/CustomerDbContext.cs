using Customer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
           : base(options)
        {
        }

        public DbSet<CustomerEntity> Customers { get; set; }
    }
}
