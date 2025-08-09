using Common.Shared.Dtos;
using StockAPI.PaymentServices;
using System.Diagnostics;
using System.Net;

namespace StockAPI.StockService;

public class StockService(PaymentService paymentService)
{
	private static Dictionary<int, int> GetProductStockList()
	{
		Dictionary<int, int> productList = [];
		productList[0] = 10;
		productList[1] = 25;
		productList[2] = 30;
		productList[3] = 43;
		return productList;
	}

	public async Task<ResponseDto<CheckAndPaymentServiceResponseDto>> CheckAndPaymentService(CheckAndPaymentServiceRequestDto requestDto)
	{
		var productStocklist = GetProductStockList();
		var stockStatus = new List<(int ProductId, bool IsStockExists)>();

		var userId = Activity.Current?.GetBaggageItem("userId");
		Activity.Current?.SetTag("userId", userId);

		foreach (var item in requestDto.OrderItems)
		{
			var isExists = productStocklist.Any(x => x.Key == item.ProductId && x.Value - item.Count >= 0);

			stockStatus.Add((item.ProductId, isExists));
		}

		if (stockStatus.Exists(x => x.IsStockExists == false))
		{
			return ResponseDto<CheckAndPaymentServiceResponseDto>
				.Fail((int)HttpStatusCode.BadRequest, "Bazı ürünlerin stokları yetersiz.");
		}

		var (isSuccess, failMessage) = await paymentService.CreatePaymentProcess(new PaymentCreateRequestDto
		{
			OrderCode = requestDto.OrderCode,
			TotalPrice = requestDto.OrderItems.Sum(x => x.UnitPrice)
		});

		if (!isSuccess)
		{
			return ResponseDto<CheckAndPaymentServiceResponseDto>
				.Fail(HttpStatusCode.BadRequest.GetHashCode(), failMessage ?? string.Empty);

		}

		return ResponseDto<CheckAndPaymentServiceResponseDto>
					.Success((int)HttpStatusCode.OK, new() { Description = "Stoklar ayrıldı." });
	}
}