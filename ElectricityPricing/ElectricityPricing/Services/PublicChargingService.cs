using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElectricityPricing.Services
{
    public class PublicChargingService : IPublicChargingService
    {
        private readonly string BaseUrl;
        private readonly HttpClient _httpClient;

        public PublicChargingService(
            HttpClient httpClient,
            IConfiguration config)
        {
            _httpClient = httpClient;
            BaseUrl = config.GetValue<string>("BaseUrls:PublicChargingBaseUrl");
        }


        public async Task<PriceAndTaxes> GetElectricityPriceForDate(DateTime dateTime)
        {

            var response = await _httpClient.GetAsync(CreateGetUrl(dateTime));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var submissionPrice = JsonConvert.DeserializeObject<PriceAndTaxes>(responseBody);

            return submissionPrice;

        }

        private string CreateGetUrl(DateTime dateTime)
        {
            var getUrl = BaseUrl + "?datetime=[DateTime]";

            var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" };
            var dateTimeString = JsonConvert.SerializeObject(dateTime, settings);
            dateTimeString = dateTimeString.Trim(new char[] { '/', '"' });
            getUrl = getUrl.Replace("[DateTime]", dateTimeString);

            Console.WriteLine("Calling public charging on " + getUrl);
            return getUrl;
        }
    }
}
