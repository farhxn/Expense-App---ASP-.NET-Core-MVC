namespace Expense_App.HelperMethods
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class StockService
    {
        private readonly HttpClient _httpClient;

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<dynamic> GetStockData(string symbol)
        {
            string apiUrl = "https://www.alphavantage.co/query?function=REALTIME_BULK_QUOTES&symbol=MSFT,AAPL,IBM&apikey=demo";

            var response = await _httpClient.GetAsync(apiUrl);

            //if (response.IsSuccessStatusCode)
            //{
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(jsonString);
            //}

            return null;
        }
    }

}
