using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ProductContext db;

        public ProductsController(ProductContext context, IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
            db = context;

            if (!db.Products.Any())
            {
                db.Products.Add(new Product { Name = "Book", Description = "Harry Potter", UserId = 1 });
                db.Products.Add(new Product { Name = "Car", Description = "Tesla", UserId = 1 });
                db.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await db.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            Product product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();
            return new ObjectResult(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            db.Products.Add(product);
            await db.SaveChangesAsync();

            DeliverMessage deliverMessage = new DeliverMessage(TypeOfRequest.PostRequest, product);

            await publishEndpoint.Publish(deliverMessage);

            return Ok(product);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> Put(Product product)

        {
            if (product == null)
            {
                return BadRequest();
            }

            if (!db.Products.Any(x => x.Id == product.Id))
            {
                return NotFound();
            }

            DeliverMessage deliverMessage = new DeliverMessage(TypeOfRequest.PutRequest, product);
            await publishEndpoint.Publish(deliverMessage);

            db.Update(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)

        {
            Product product = db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            DeliverMessage deliverMessage = new DeliverMessage(TypeOfRequest.DeleteRequest, product);
            await publishEndpoint.Publish(deliverMessage);

            return Ok(product);
        }
    }
}
