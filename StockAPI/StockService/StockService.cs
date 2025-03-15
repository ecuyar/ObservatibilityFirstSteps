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

		public async Task CheckAndPaymentService(CheckAndPaymentServiceRequestDto requestDto)
		{
			var productStocklist = GetProductStockList();
			var stockStatus = new List<(int productId, bool isStockExists)>();

		}
	}
}
