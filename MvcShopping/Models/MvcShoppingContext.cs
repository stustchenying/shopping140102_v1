using System.Data.Entity;

namespace MvcShopping.Models
{
    public class MvcShoppingContext : DbContext
    {
        public MvcShoppingContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<OrderHeader> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetailItems { get; set; }
    }
}