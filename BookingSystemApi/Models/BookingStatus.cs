using System;
using System.Collections.Generic;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class BookingStatus
    {
        public BookingStatus()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
