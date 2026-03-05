namespace projectApiAngular.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using projectApiAngular.DTO;
    using projectApiAngular.Services;
    using static projectApiAngular.DTO.PurcheseDto;

    [ApiController]
    [Route("api/[controller]")]
    public class PurcheseController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurcheseController(IPurchaseService service)
        {
            _service = service;
        }

        // add
        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<ActionResult<ReadPurcheseDto>> AddPurchase([FromBody] CreatePurcheseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.AddPurchaseAsync(dto);
                return Created(string.Empty, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        //GetBuyersDetails
        [Authorize(Roles = "admin")]
        [HttpGet("buyers")]
        public async Task<ActionResult<IEnumerable<ReadPurcheseDto>>> GetBuyersDetails()
        {
            var result = await _service.GetBuyersDetails();
            return Ok(result);
        }

        // GetGiftsSortedBySales
        [Authorize(Roles = "admin")]
        [HttpGet("gifts/sorted")]
        public async Task<ActionResult<IEnumerable<ReadPurcheseDto>>> GetGiftsSortedBySales()
        {
            var result = await _service.GetGiftsSortedBySalesAsync();
            return Ok(result);
        }

        // GetPurchasesByGift
        [Authorize(Roles = "admin")]
        [HttpGet("gift/{name}")]
        public async Task<ActionResult<IEnumerable<ReadPurcheseDto>>> GetPurchasesByGift(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Gift name is required.");

            var result = await _service.GetPurchasesByGiftAsync(name);
            return Ok(result);
        }

        // GetPurchasesOrderedByPrice
        [Authorize(Roles = "admin")]
        [HttpGet("ordered-by-price")]
        public async Task<ActionResult<IEnumerable<ReadPurcheseDto>>> GetPurchasesOrderedByPrice()
        {
            var result = await _service.GetPurchasesOrderedByPriceAsync();
            return Ok(result);
        }

        [HttpGet("total-revenue")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<decimal>> GetTotalSalesRevenue()
        {
            var totalRevenue = await _service.GetTotalSalesRevenue();
            return Ok(totalRevenue);
        }
    }
}
