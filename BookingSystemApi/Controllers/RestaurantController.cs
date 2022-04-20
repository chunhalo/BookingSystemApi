using BookingSystemApi.Authentication;
using BookingSystemApi.Interface;
using BookingSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IResRepo _resRepo;
        public RestaurantController(IResRepo resRepo)
        {
            _resRepo = resRepo;
        }

        [HttpGet]
        [Route("GetAllRestaurants")]
        public async Task<ActionResult<PagedList<List<ReturnResWithStatus>>>> GetAllRestaurants([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnResWithStatus>> pagedList = await _resRepo.GetAllRestaurants(paginationFilter);
            return pagedList;
        }

        [HttpGet]
        [Route("ActiveRestaurants")]
        public async Task<ActionResult<PagedList<List<Restaurant>>>> GetActiveRestaurants([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<Restaurant>> pagedList = await _resRepo.GetActiveRestaurants(paginationFilter);
            return pagedList;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurantById(int id)
        {
            Restaurant res = await _resRepo.GetRestaurantById(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);

        }


        [HttpGet]
        [Route("GetRestaurantStatuses")]
        public async Task<ActionResult<List<BookingStatus>>> GetRestaurantStatuses()
        {
            var restaurantStatuses = await _resRepo.GetRestaurantStatuses();
            return Ok(restaurantStatuses);
        }

        [HttpGet]
        [Route("GetAllRestaurantStatuses")]
        public async Task<ActionResult<List<BookingStatus>>> GetAllRestaurantStatuses()
        {
            var restaurantStatuses = await _resRepo.GetAllRestaurantStatuses();
            return Ok(restaurantStatuses);
        }
        [HttpPost]
        public async Task<ActionResult<int>> PostRes([FromForm] ResAddModel res)
        {

             int resId = await _resRepo.AddRes(res);
            if (resId != 0)
            {
                return Ok(resId);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromForm] RestaurantUpdateRequest restaurantUpdateRequest)
        {

            await _resRepo.UpdateRestaurant(restaurantUpdateRequest);
            return Ok();
        }


    }


}
