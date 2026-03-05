using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectApiAngular.Services;
using static projectApiAngular.DTO.BasketDto;

namespace projectApiAngular.Controllers
{
    [Authorize (Roles ="user")] 
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }


      
        //get my basket
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<ReadBasketDto>>> GetMyBasket()
        {
            try
            {
                var baskets = await _basketService.GetMyBasket();
                return Ok(baskets);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // enter to basket
        [HttpPost]
        public async Task<ActionResult<ReadBasketDto>> EnterToBasket([FromBody] CreateBasketDto basketDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _basketService.EnterToBasketAsync(basketDto);
                return Created(string.Empty, created);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        // update amount
        [HttpPut("{id}/amount")]
        public async Task<ActionResult<ReadBasketDto?>> UpdateAmount(int id, [FromQuery] int newAmount)
        {
            if (newAmount <= 0 || newAmount > 1000)
                return BadRequest("Amount must be greater than zero and cannot exceed 1000.");

            try
            {
                var updated = await _basketService.UpdateBasketAmountAsync(id, newAmount);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        //delete basket
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReadBasketDto?>> Delete(int id)
        {
            try
            {
                var deleted = await _basketService.DeleteBasketAsync(id);
                if (deleted == null) return NotFound();
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        //BuyAllBasket
        [HttpPost("buy-all")]
        public async Task<IActionResult> BuyAll()
        {
            var success = await _basketService.BuyAllBasket();

            if (success)
            {
                return Ok(new { message = "הרכישה בוצעה בהצלחה!" });
            }

            return BadRequest(new { message = "לא ניתן היה להשלים את הרכישה. וודא שהסל אינו ריק." });
        }
    }
}
