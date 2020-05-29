using ElectricityPricing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Threading.Tasks;


namespace ElectricityPricing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PriceController : ControllerBase
    {


        private readonly ILogger<PriceController> _logger;
        private readonly IPublicChargingService _publicChargingService;

        public PriceController(
            ILogger<PriceController> logger,
            IPublicChargingService publicChargingService)
        {
            _logger = logger;
            _publicChargingService = publicChargingService;
        }


        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] DateTime dateTime,
            [FromQuery] double ressourceUsage,
            [FromQuery] string unitOfMeassure)
        {
            var priceRequest = new PriceRequest()
            {
                DateTime = dateTime,
                RessourceUsage = ressourceUsage,
                UnitOfMeassure = unitOfMeassure
            };

            var totalPrice = await CalculateSubmissionPrice(priceRequest);

            return Ok(totalPrice);
        }

        private async Task<SubmissionPrice> CalculateSubmissionPrice(PriceRequest priceRequest)
        {
            var electricityPriceInfo = await _publicChargingService.GetElectricityPriceForDate(priceRequest.DateTime);
            var price = priceRequest.RessourceUsage * (electricityPriceInfo.Price + electricityPriceInfo.Tax);

            var submissionPrice = new SubmissionPrice()
            {
                Currency = electricityPriceInfo.Currency,
                TotalCost = price,
            };

            return submissionPrice;
        }
    }
}
