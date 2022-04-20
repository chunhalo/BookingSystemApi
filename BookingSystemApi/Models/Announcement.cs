using System;
using System.Collections.Generic;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        public virtual AspNetUser User { get; set; }
    }

    public class AnnouncementAddModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
    }

    public class ReturnAnnouncement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }

    }
}
