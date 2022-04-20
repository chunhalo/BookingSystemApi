using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class Table
    {
        public Table()
        {
            Bookings = new HashSet<Booking>();
        }

        public int TableId { get; set; }
        public int ResId { get; set; }
        public int Accommodate { get; set; }
        public int TableNo { get; set; }
        public int Status { get; set; }
        public virtual Restaurant Res { get; set; }
        public virtual TableStatus StatusNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
    public class BookDateWithResId
    {
        public int resId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}
