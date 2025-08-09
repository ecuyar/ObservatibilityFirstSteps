using Common.Shared.Dtos;

namespace StockAPI.PaymentServices;

public class PaymentService(HttpClient httpClient)
{
	private const string PAYMENT_CREATE_ENDPOINT = "api/PaymentProcess/Create";

	public async Task<(bool isSuccess, string? failMessage)> CreatePaymentProcess(PaymentCreateRequestDto requestDto)
	{
		var response = await httpClient.PostAsJsonAsync(PAYMENT_CREATE_ENDPOINT, requestDto);
		var responseContent = await response.Content.ReadFromJsonAsync<ResponseDto<PaymentCreateResponseDto>>();

		return response.IsSuccessStatusCode ? (true, null) : (false, responseContent?.Errors?.First());
	}
}
