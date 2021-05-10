using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolSystemAPI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoolSystemAPI.Ticket
{
    /// <summary>
    /// Endpoint for interacting with Tickets when leaving
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExitController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        public ExitController(
            ITicketRepository ticketRepo,
            IMapper mapper,
            ILoggerService logger)
        {
            _ticketRepo = ticketRepo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// call to log exit time
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateEntry(int ticketId)
        {
            var controllerName = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{controllerName}: Attempted Call - TicketId: {ticketId}");
                var item = await _ticketRepo.FindByTicketId(ticketId);
                if (item.Charges == 0)
                {
                    item.Active = false;
                }
                item.ExitTime = DateTime.Now;
                var isSuccess = await _ticketRepo.Update(item);
                if (isSuccess == false)
                {
                    return InternalError($"{controllerName}: Update failed - TicketId: {ticketId}");
                }
                _logger.LogInfo($"{controllerName}: Update Successful - TicketId: {ticketId}");
                return Ok(item);
            }
            catch (Exception Ex)
            {
                return InternalError($"{controllerName}: {Ex.Message} - {Ex.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong, please contact the administrator");
        }
    }
}
