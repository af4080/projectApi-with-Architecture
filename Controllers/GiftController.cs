using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using projectApiAngular.Services;
using static projectApiAngular.DTO.GiftDto;

namespace projectApiAngular.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }
        //get
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllGifts()
        {
            var gifts = await _giftService.GetAllGifts();
            return Ok(gifts);
        }
        //get by name
        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<IActionResult> GetGiftByName(string name)
        {
            var gift = await _giftService.GetGiftByName(name);
            if (gift == null)
                return NotFound();
            return Ok(gift);
        }
        //get by doner
        [HttpGet("doner/{name}")]
        public async Task<IActionResult> GetGiftByDonnerName(string name)
        {
            var gifts = await _giftService.GetGiftByDonnerName(name);
            return Ok(gifts);
        }
        //get by num customer
        [HttpGet("numcustomer/{count}")]
        public async Task<IActionResult> GetbyNumCastomer(int count)
        {
            var gifts = await _giftService.GetbyNumCustomer(count);
            return Ok(gifts);
        }

        //post
        [HttpPost]
        public async Task<IActionResult> Addgift([FromBody] CreateGiftDto gift)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var g = await _giftService.AddGift(gift);
                return CreatedAtAction(nameof(GetGiftByName), new { name = g.Name }, g);

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }


        }
        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            try
            {
                var deletedGift = await _giftService.DeleteGift(id);
                if (deletedGift == null)
                    return NotFound();
                return Ok(deletedGift);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //update
        [HttpPatch("{name}")]
        public async Task<IActionResult> UpdateGift([FromRoute] string name, [FromBody] UpdateGiftDto gift)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updatedGift = await _giftService.UpdateGift(name, gift);
                if (updatedGift == null)
                    return NotFound();
                return Ok(updatedGift);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
        [AllowAnonymous]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedGifts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var response = await _giftService.GetPagedGifts(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("{giftId}/winner")]
        public async Task<IActionResult> GetWinnerByGiftId(int giftId)
        {
            try
            {
                var winner = await _giftService.GetWinnerByGiftId(giftId);
                if (winner == null)
                    return NotFound();
                return Ok(new { winner });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
