using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoolSystemAPI.Ticket
{
    public class TicketDTO
    {
        public int TicketId { get; set; }
        public int PoolId { get; set; }
        public int Charges { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
    }
}
