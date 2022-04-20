using BookingSystemApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystemApi.Interface
{
    public interface IAnnounceRepo
    { 
        Task PostAnnouncement(AnnouncementAddModel announcementAddModel, string username);
        Task<PagedList<List<ReturnAnnouncement>>> GetAllAnnouncement(PaginationFilter paginationFilter);
    }
}
