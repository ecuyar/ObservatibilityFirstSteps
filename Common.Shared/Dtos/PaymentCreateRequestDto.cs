namespace Common.Shared.Dtos
{
	public record PaymentCreateRequestDto
	{
		public string OrderCode { get; set; } = string.Empty;
		public decimal TotalPrice { get; set; }
	}
}
