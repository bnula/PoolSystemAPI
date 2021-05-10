using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PoolSystemAPI.Ticket;

namespace PoolSystemAPI.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Ticket.Ticket, TicketDTO>().ReverseMap();
        }
    }
}
