using MassTransit;
using Microsoft.Extensions.Logging;
using Model;
using System.Linq;
using System.Threading.Tasks;

namespace ControleService
{
    internal class ProductConsumer : IConsumer<DeliverMessage>
    {
        private readonly ILogger<ProductConsumer> logger;
        private readonly ProductContextCopy db;

        public ProductConsumer(ProductContextCopy context,ILogger<ProductConsumer> logger)
        {
            this.logger = logger;
            db = context;

            if (!db.Products.Any())
            {
                db.Products.Add(new Product { Name = "Book", Description = "Harry Potter", UserId = 1 });
                db.Products.Add(new Product { Name = "Car", Description = "Tesla", UserId = 1 });
                db.SaveChanges();
            }
        }

        public ILogger<ProductConsumer> Logger { get; }

        public async Task Consume(ConsumeContext<DeliverMessage> context)
        {
            TypeOfRequest requestType = context.Message.typeOfRequest;

            switch (requestType)
            {
                case TypeOfRequest.PostRequest:
                    db.Products.Add(new Product { 
                        Name = context.Message.product.Name, 
                        Description = context.Message.product.Description, 
                        UserId = context.Message.product.UserId 
                    });
                    break;

                case TypeOfRequest.PutRequest:
                    db.Update(context.Message.product);
                    break;

                case TypeOfRequest.DeleteRequest:
                    Product product = db.Products.FirstOrDefault(x => x.Id == context.Message.product.UserId);
                    db.Products.Remove(product);
                    break;
            }

            await db.SaveChangesAsync();
        }

    }

}
