using BookingSystemApi.Authentication;
using BookingSystemApi.Interface;
using BookingSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingSystemApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookRepo _bookRepo;

        public BookingController(IBookRepo bookRepo)
        {
            _bookRepo = bookRepo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReturnBookingWithIntStatus>> GetBookingById(int id)
        {
            ReturnBookingWithIntStatus booking = await _bookRepo.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);

        }

        [HttpPost]
        [Route("BookingWithIdPageList")]
        public async Task<ActionResult<PagedList<List<ReturnPendingBooking>>>> GetBookingByIdWithPageList([FromForm] string searchText,[FromForm]string choice,[FromQuery] PaginationFilter paginationFilter)
        {
            if (choice == "bookId")
            {
                PagedList<List<ReturnPendingBooking>> pagedList = await _bookRepo.GetBookingByIdWithPageList(searchText, paginationFilter);
                return pagedList;
            }
            else
            {
                PagedList<List<ReturnPendingBooking>> pagedList = await _bookRepo.GetBookingByUsernameWithPageList(searchText, paginationFilter);
                return pagedList;
            }
        }

        [AllowAnonymous]
        [EnableCors("AnotherPolicy")]
        [HttpPut]
        [Route("UpdateDateTime")]
        public async Task<ActionResult<CheckExistBooking>> UpdateBookingDateTime([FromForm] UpdateBookingDateTime updateBookingDateTime)
        {
            var checkExisting = await _bookRepo.UpdateBookingDateTime(updateBookingDateTime);
            return Ok(checkExisting);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("UpdateRequestStatus")]
        public async Task<ActionResult> UpdateBookingRequestStatus([FromForm] UpdateBookingRequestStatus updateBookingRequestStatus)
        {
            await _bookRepo.UpdateBookingRequestStatus(updateBookingRequestStatus);
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("PendingBooking")]
        public async Task<ActionResult<PagedList<List<ReturnPendingBooking>>>> GetPendingBooking([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnPendingBooking>> pagedList = await _bookRepo.GetPendingBooking(paginationFilter);
            return pagedList;
        }

        [HttpGet]
        [Route("GetBookingStatus")]
        public async Task<ActionResult<List<BookingStatus>>> GetBookingStatus()
        {
            var bookingstatuses = await _bookRepo.GetBookingStatuses();
            return Ok(bookingstatuses);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("SearchPending")]
        public async Task<ActionResult<PagedList<List<ReturnPendingBooking>>>> SearchPendingBooking([FromForm] string searchText, [FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnPendingBooking>> pagedList = await _bookRepo.searchPendingBooking(searchText,paginationFilter);
            return pagedList;
        }

        [HttpGet]
        [Route("Allbooking")]
        public async Task<ActionResult<PagedList<List<ReturnPendingBooking>>>> GetBookingWithPageList([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnPendingBooking>> pagedList = await _bookRepo.GetBookingWithPageList(paginationFilter);
            return pagedList;
        }



        [HttpPost]
        public async Task<ActionResult<Response>> AddBooking([FromForm]AddBookingModel addBookingModel)
        {
            var identity = User.Identity as ClaimsIdentity;
            Response getresponse = new Response();
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);

                getresponse =await _bookRepo.AddBooking(addBookingModel, username);

            }
            
            return Ok(getresponse);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("SetConfirmBooking")]
        public async Task<ActionResult> SetConfirmBooking([FromForm] int BookId)
        {
            var response = await _bookRepo.SetConfirmBooking(BookId);
            if (response.Status=="Modified" || response.Status=="Success")
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
            
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("SetConfirmDate")]
        public async Task<ActionResult> SetConfirmDate([FromForm] int BookId)
        {
            var response = await _bookRepo.SetConfirmDate(BookId);
            if (response.Status == "Modified" || response.Status == "Success")
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }

        }

        [HttpPut]
        [Route("CancelBooking")]
        public async Task<ActionResult> CancelBooking([FromForm] int BookId)
        {
            await _bookRepo.CancelBooking(BookId);
            return Ok();


        }
    }
}
