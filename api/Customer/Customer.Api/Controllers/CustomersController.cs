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
            [FromQuery] int size,
            [FromQuery] string? sort)
        {
            var sizeWithDefault = size == 0 ? 20 : size;
            var offset = page * size;
            var customers = await customerRepository.List(name, offset, sizeWithDefault, sort);
            var count = await customerRepository.Count(name);
            var result = customers.Select(x => ExpandSingleItem(x));

            return Ok(new
            {
                _embedded = new {
                    customers = result
                },
                _links = CreateLinksForCollection(page, sizeWithDefault, sort, count),
                page = new
                {
                    size = size,
                    totalElements = count,
                    totalPages = (int)Math.Ceiling((float)count / (float)sizeWithDefault),
                    number = page,
                }
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
        
        private dynamic CreateLinksForCollection(int page, int size, string sort, int count)
        {
            dynamic links = new ExpandoObject();

            links.self = new
            {
                href = $"{linkGenerator.GetUriByAction(HttpContext, nameof(List))}{{&sort,page,size}}",
                templated = true,
            };
            
            links.create = new Link(linkGenerator.GetUriByAction(HttpContext, nameof(Add)));

            if ((page + 1) * size < count)
            {
                links.next = new
                {
                    href = $"{linkGenerator.GetUriByAction(HttpContext, nameof(List))}?page={page + 1}&size={size}{{&sort}}",
                    templated = true,
                };
            }

            if (page > 0)
            {
                links.prev = new
                {
                    href = $"{linkGenerator.GetUriByAction(HttpContext, nameof(List))}?page={page - 1}&size={size}{{&sort}}",
                    templated = true,
                };
            }

            return links;
        }

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