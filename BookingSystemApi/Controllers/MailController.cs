using AnnouncementLibrary;
using BookingSystemApi.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailRepo _mailRepo;

        public MailController(IMailRepo mailRepo)
        {
            _mailRepo = mailRepo;
        }

        [HttpPut]
        [Route("Announcement")]
        public async Task<ActionResult> SendAnnouncement([FromForm] AnnouncementClass announcement)
        {
            await _mailRepo.SendAnnouncement(announcement);
            return Ok();

        }

        [HttpPut]
        [Route("PendingMail")]
        public async Task<ActionResult> SendPendingMail([FromForm] BookingClass booking)
        {
            await _mailRepo.SendPendingMail(booking);
            return Ok();

        }

        [HttpPut]
        [Route("ConfirmMail")]
        public async Task<ActionResult> SendConfirmMail([FromForm] BookingClass booking)
        {
            await _mailRepo.SendConfirmMail(booking);
            return Ok();

        }
    }
}
