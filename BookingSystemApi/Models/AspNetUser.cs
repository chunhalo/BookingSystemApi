using System;
using System.Collections.Generic;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            Announcements = new HashSet<Announcement>();
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Bookings = new HashSet<Booking>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }

    public class UserDetailStatus
    {
        public string username { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        //public IEnumerable<string> roles { get; set; }
        public string roles { get; set; }
        public string status { get; set; }
    }
    public class returnUserDetailStatus
    {
        public string username { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string status { get; set; }
    }

    public class UserUpdate
    {
        public string old_password { get; set; }
        public string password { get; set; }
    }
}
