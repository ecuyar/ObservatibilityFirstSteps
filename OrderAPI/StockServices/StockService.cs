using Common.Shared.Dtos;

namespace OrderAPI.StockServices;

public class StockService(HttpClient client)
{
	private const string CHECK_AND_PAYMENT_ENDPOINT = "/api/Stock/CheckAndStartPayment";

	public async Task<(bool isSuccess, List<string>? failMessage)> CheckAndPaymentAsync(CheckAndPaymentServiceRequestDto requestDto)
	{
		var response = await client.PostAsJsonAsync<CheckAndPaymentServiceRequestDto>(CHECK_AND_PAYMENT_ENDPOINT, requestDto);
		var content = await response.Content.ReadFromJsonAsync<ResponseDto<CheckAndPaymentServiceResponseDto>>();

		return response.IsSuccessStatusCode ? (true, null) : (false, content?.Errors);
	}
}
