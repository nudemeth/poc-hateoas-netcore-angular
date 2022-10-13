using Customer.Api.Entities;
using Customer.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text.Json;

namespace Customer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;
        private readonly LinkGenerator linkGenerator;
        private readonly ILogger<CustomersController> logger;

        public CustomersController(ICustomerRepository customerRepository, LinkGenerator linkGenerator, ILogger<CustomersController> logger)
        {
            this.customerRepository = customerRepository;
            this.linkGenerator = linkGenerator;
            this.logger = logger;
        }

        [HttpGet(Name = nameof(List))]
        public async Task<IActionResult> List(
            [FromQuery] string? name,
            [FromQuery] int page,
            [FromQuery] int limit,
            [FromQuery] string? sort)
        {
            var pageWithDefault = page == 0 ? 1 : page;
            var limitWithDefault = limit == 0 ? 20 : limit;
            var offset = pageWithDefault == 1 ? 0 : (pageWithDefault - 1) * limitWithDefault;
            var customers = await customerRepository.List(name, offset, limitWithDefault, sort);
            var count = await customerRepository.Count(name);

            var paginationMetadata = new
            {
                totalCount = count,
                pageSize = limit,
                currentPage = page,
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var toReturn = customers.Select(x => ExpandSingleItem(x));

            return Ok(new
            {
                _embedded = new {
                    customers = toReturn
                },
                _links = new
                {
                    self = new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List))),
                    create = new Link(linkGenerator.GetUriByAction(HttpContext, nameof(Add))),
                    first = new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List))),
                    next = "",
                    previous = "",
                },
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(Get))]
        public async Task<IActionResult> Get(int id)
        {
            CustomerEntity customer = await customerRepository.Get(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleItem(customer));
        }

        [HttpPost(Name = nameof(Add))]
        public async Task<IActionResult> Add([FromBody] CustomerEntity customer)
        {
            customer.CreatedAt = DateTime.Now;
            await customerRepository.Add(customer);
            await customerRepository.Save();

            return CreatedAtRoute(nameof(Get), new { id = customer.Id }, customer);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await customerRepository.Get(id);

            if (customer == null)
            {
                return NotFound();
            }

            await customerRepository.Delete(customer);
            await customerRepository.Save();

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(Update))]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerEntity customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var existingCustomer = await customerRepository.Get(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            existingCustomer.Name = customer.Name;

            await customerRepository.Update(existingCustomer);
            await customerRepository.Save();

            return Ok(ExpandSingleItem(existingCustomer));
        }
        /*
        private List<Link> CreateLinksForCollection(string name, int page, int limit, string sortBy, int count)
        {
            var links = new List<Link>();

            links.Add(
             new Link(linkGenerator.GetUriByAction(HttpContext, nameof(Add))));

            // self 
            links.Add(
             new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List), values: new
             {
                 name = name,
                 page = page,
                 limit = limit,
                 sortBy = sortBy
             }), "self", "GET"));

            links.Add(new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List), values: new
            {
                name = name,
                page = 1,
                limit = limit,
                sortBy = sortBy
            }), "first", "GET"));

            if (page * limit < count)
            {
                links.Add(new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List), values: new
                {
                    name = name,
                    page = page + 1,
                    limit = limit,
                    sortBy = sortBy
                }), "next", "GET"));
            }

            if (page > 1)
            {
                links.Add(new Link(linkGenerator.GetUriByAction(HttpContext, nameof(List), values: new
                {
                    name = name,
                    page = page - 1,
                    limit = limit,
                    sortBy = sortBy
                }), "previous", "GET"));
            }

            return links;
        }*/

        private dynamic ExpandSingleItem(CustomerEntity customer)
        {
            dynamic customerWithLinks = new ExpandoObject();
            var link = GetLinks(customer.Id);
            var dictionary = (IDictionary<string, object>)customerWithLinks;

            foreach (var property in customer.GetType().GetProperties())
            {
                dictionary.Add(JsonNamingPolicy.CamelCase.ConvertName(property.Name), property.GetValue(customer));
            }

            customerWithLinks._links = new
            {
                self = link,
                delete = link,
                update = link,
            };

            return customerWithLinks;
        }

        private Link GetLinks(int id)
        {
            return new Link(linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id = id }));
        }
    }
}