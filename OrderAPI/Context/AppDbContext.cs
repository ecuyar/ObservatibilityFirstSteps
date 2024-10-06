using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Context
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
	}
}
