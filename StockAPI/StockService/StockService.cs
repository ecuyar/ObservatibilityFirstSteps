using Common.Shared;
using System.Net;

namespace StockAPI.StockService
{
	public class StockService
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

		public ResponseDto<CheckAndPaymentServiceResponseDto> CheckAndPaymentService(CheckAndPaymentServiceRequestDto requestDto)
		{
			var productStocklist = GetProductStockList();
			var stockStatus = new List<(int ProductId, bool IsStockExists)>();

			foreach (var item in requestDto.OrderItems)
			{
				var isExists = productStocklist.Any(x => x.Key == item.ProductId && x.Value > 0);

				stockStatus.Add((item.ProductId, isExists));
			}

			if (stockStatus.Exists(x => x.IsStockExists == false))
			{
				return ResponseDto<CheckAndPaymentServiceResponseDto>
					.Fail((int)HttpStatusCode.BadRequest, "Bazı ürünlerin stokları yetersiz.");
			}

			return ResponseDto<CheckAndPaymentServiceResponseDto>
					.Success((int)HttpStatusCode.OK, new() { Description = "Stoklar ayrıldı." });

			//payment süreci devam edecek

		}
	}
}