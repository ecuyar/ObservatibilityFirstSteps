namespace Common.Shared.Dtos
{
	public record CheckAndPaymentServiceRequestDto
	{
		public string OrderCode { get; set; } = null!;
		public List<CreateOrderItemRequestDto> OrderItems { get; set; } = [];
	}
}
