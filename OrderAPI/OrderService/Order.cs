namespace OrderAPI.OrderService
{
	public class Order
	{
		public int Id { get; set; }
		public string OrderCode { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public int CreatedBy { get; set; }
		public OrderStatus Status { get; set; }

		//Navigation properties
		public List<OrderItems> Items { get; set; } = null!;
	}

	public class OrderItems
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Count { get; set; }
		public decimal UnitPrice { get; set; }

		//Navigation properties
		public int OrderId { get; set; }
		public Order Order { get; set; } = null!;
	}

	public enum OrderStatus : byte
	{
		Success = 1,
		Fail = 0
	}
}
