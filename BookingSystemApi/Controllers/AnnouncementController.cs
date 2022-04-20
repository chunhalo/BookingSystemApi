using BookingSystemApi.Authentication;
using BookingSystemApi.Interface;
using BookingSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnounceRepo _announceRepo;

        public AnnouncementController(IAnnounceRepo announceRepo)
        {
            _announceRepo = announceRepo;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("AllAnnouncement")]
        public async Task<ActionResult<PagedList<List<ReturnAnnouncement>>>> GetAllAnnouncement([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnAnnouncement>> pagedList = await _announceRepo.GetAllAnnouncement(paginationFilter);
            return pagedList;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult> PostAnnouncement([FromForm] AnnouncementAddModel announcementAddModel)
        {
            var identity = User.Identity as ClaimsIdentity;
           
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);

                await _announceRepo.PostAnnouncement(announcementAddModel,username);

            }
            return Ok();
        }
    }

}
