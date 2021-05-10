using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoolSystemAPI.Ticket
{
    public class Ticket
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int? PoolId { get; set; }
        public int Charges { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public bool Active { get; set; }
    }
}
