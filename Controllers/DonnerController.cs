using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectApiAngular.Services;
using static projectApiAngular.DTO.DonnerDto;

namespace projectApiAngular.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DonnerController: ControllerBase
    {
       private readonly IDonnerService _donnerService;

         public DonnerController(IDonnerService donnerService)
         {
              _donnerService = donnerService;
         }
        //get
        [HttpGet]
        public async Task<IActionResult> GetAllDonners()
        {
            var donners = await _donnerService.GetAllDonners();
            return Ok(donners);
        }

        //get by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonnerById([FromRoute] int id)
        {
            var donner = await _donnerService.GetDonnerById(id);
            if (donner == null)
            {
                return NotFound();
            }
            return Ok(donner);
        }

        //get by name
        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetDonnerByName( [FromRoute]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("name is required");

            var donner = await _donnerService.GetDonnerByName(name);
            if (donner == null)
            {
                return NotFound();
            }
            return Ok(donner);
        }

        //get by email
        [HttpGet("byemail")]
        public async Task<IActionResult> GetDonnerByEmail( [FromQuery]string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("email is required");

            var donner = await _donnerService.GetDonnerByEmail(email);
            if (donner == null)
                return NotFound();

            return Ok(donner);
   
        }
        //get by gift id
        [HttpGet("bygift/{giftId}")]
        public async Task<IActionResult> GetDonnerByGiftId(int giftId)
        {
            var donner = await _donnerService.GetDonnerByGiftId(giftId);
            if (donner == null)
            {
                return NotFound();
            }
            return Ok(donner);
        }

        //post
        [HttpPost]
        public async Task<IActionResult> AddDonner([FromBody] CreateDonnerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var addedDonner = await _donnerService.AddDonner(dto);
                return CreatedAtAction(nameof(GetDonnerById), new { id = addedDonner.Id }, addedDonner);

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });

            }
        }
        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonner([FromRoute] int id)
        {
            var deletedDonner = await _donnerService.DeleteDonner(id);
            if (deletedDonner == null)
            {
                return NotFound();
            }
            return Ok(deletedDonner);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonner([FromRoute] int id, [FromBody] UpdateDonnerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedDonner = await _donnerService.UpdateDonner(id, dto);
            if (updatedDonner == null)
            {
                return NotFound();
            }
            return Ok(updatedDonner);
        }




    }
}
