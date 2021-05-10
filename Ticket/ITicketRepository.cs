using PoolSystemAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoolSystemAPI.Ticket
{
    public interface ITicketRepository : IRepositoryBase<Ticket>
    {
        Task<Ticket> FindByTicketId(int ticketId);
    }
}
