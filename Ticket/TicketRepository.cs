using Microsoft.EntityFrameworkCore;
using PoolSystemAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoolSystemAPI.Ticket
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _db;

        public TicketRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Ticket entity)
        {
            await _db.AddAsync(entity);
            return await Save();
        }

        public Task<bool> Delete(Ticket entity)
        {
            _db.Remove(entity);
            return Save();
        }

        public async Task<IList<Ticket>> FindAll()
        {
            var items = await _db.Tickets.ToListAsync();
            return items;
        }

        public async Task<Ticket> FindById(int id)
        {
            var item = await _db.Tickets.FindAsync(id);
            return item;
        }

        public async Task<Ticket> FindByTicketId(int ticketId)
        {
            var item = await _db.Tickets
                .OrderByDescending(s => s.EntryTime)
                .FirstOrDefaultAsync(s => s.TicketId == ticketId);
            return item;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Ticket entity)
        {
            _db.Update(entity);
            return await Save();
        }
    }
}
