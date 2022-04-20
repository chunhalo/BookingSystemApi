using System;
using System.Collections.Generic;

#nullable disable

namespace BookingSystemApi.Models
{
    public partial class TableStatus
    {
        public TableStatus()
        {
            Tables = new HashSet<Table>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<Table> Tables { get; set; }
    }
}
