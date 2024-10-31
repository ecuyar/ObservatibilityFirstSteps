using Microsoft.EntityFrameworkCore;
using OrderAPI.OrderService;

namespace OrderAPI.Context
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
	}
}
