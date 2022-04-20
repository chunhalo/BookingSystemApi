using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            Bookings = new HashSet<Booking>();
            Tables = new HashSet<Table>();
        }

        public int ResId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string OperationStart { get; set; }
        public string OperationEnd { get; set; }

        public virtual RestaurantStatus StatusNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }
        [JsonIgnore]
        public virtual ICollection<Table> Tables { get; set; }
    }

    public class ResAddModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string OperationStart { get; set; }
        public string OperationEnd { get; set; }
    }

    public class ReturnResWithStatus
    {
        public int ResId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public RestaurantStatus restaurantStatus { get; set; }
        public string Description { get; set; }
        public string OperationStart { get; set; }
        public string OperationEnd { get; set; }
    }

    public class RestaurantUpdateRequest
    {
        public int ResId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string OperationStart { get; set; }
        public string OperationEnd { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
