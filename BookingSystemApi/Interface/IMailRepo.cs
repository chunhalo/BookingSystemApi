using AnnouncementLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Interface
{
    public interface IMailRepo
    {
        Task SendAnnouncement(AnnouncementClass announcement);
        Task SendPendingMail(BookingClass booking);
        Task SendConfirmMail(BookingClass booking);
    }
}
