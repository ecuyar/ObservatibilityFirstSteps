namespace Common.Shared.Dtos
{
	public record CreateOrderRequestDto
	{
		public required int UserId { get; set; }
		public required List<CreateOrderItemRequestDto> Items { get; set; }
	}

	public record CreateOrderItemRequestDto
	{
		public required int ProductId { get; set; }
		public required int Count { get; set; }
		public required decimal UnitPrice { get; set; }
	}
}
