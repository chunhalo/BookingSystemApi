using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class Booking
    {
        public int BookId { get; set; }
        public int ResId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        [ConcurrencyCheck]
        public int Status { get; set; }
        public DateTime EndDate { get; set; }
        public int TableId { get; set; }
        public string Request { get; set; }

        public virtual Restaurant Res { get; set; }
        public virtual BookingStatus StatusNavigation { get; set; }
        public virtual Table Table { get; set; }
        public virtual AspNetUser User { get; set; }
    }
    public class CheckExistBooking
    {
        public bool checkExist { get; set; }
    }



    public class AddBookingModel
    {
        public int ResId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int TableId { get; set; }
        public string Request { get; set; }
    }
    public class ReturnPendingBooking
    {
        public int bookingId { get; set; }
        public string resName { get; set; }
        public string username { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public int tableNo { get; set; }
        public string request { get; set; }
    }

    public class ReturnBookingWithIntStatus
    {
        public int bookingId { get; set; }
        public Restaurant res { get; set; }
        public string username { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int status { get; set; }
        public Table table { get; set; }
        public string request { get; set; }
    }

    public class UpdateBookingDateTime
    {
        public int bookingId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int tableId { get; set; }
    }

    public class UpdateBookingRequestStatus
    {
        public int bookingId { get; set; }
        public string request { get; set; }
        public int StatusId { get; set; }
    }
}
