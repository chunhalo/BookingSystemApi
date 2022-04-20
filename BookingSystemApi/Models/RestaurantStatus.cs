using System;
using System.Collections.Generic;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class RestaurantStatus
    {
        public RestaurantStatus()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
