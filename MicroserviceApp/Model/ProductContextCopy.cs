using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class ProductContextCopy : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductContextCopy(DbContextOptions<ProductContextCopy> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
