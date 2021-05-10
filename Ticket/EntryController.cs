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
    /// Endpoint for interacting with Tickets when entering
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        public EntryController(
            ITicketRepository ticketRepo,
            IMapper mapper,
            ILoggerService logger)
        {
            _ticketRepo = ticketRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{ticketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTicket(int ticketId)
        {
            var controllerName = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{controllerName}: Attempted Call - TicketId: {ticketId}");
                var item = await _ticketRepo.FindByTicketId(ticketId);

                if (item == null)
                {
                    _logger.LogWarn($"{controllerName}: Not Found - TicketId: {ticketId}");
                    return NotFound();
                }

                var response = _mapper.Map<TicketDTO>(item);
                return Ok(response);
            }
            catch (Exception Ex)
            {
                return InternalError($"{controllerName}: {Ex.Message} - {Ex.InnerException}");
            }
        }

        /// <summary>
        /// Call to validate and update ticket record based on ticketId
        /// </summary>
        /// <param name="Ticket"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpPut("{ticketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEntry([FromBody] TicketDTO itemDTO, int ticketId)
        {
            var controllerName = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{controllerName}: Attempted Call - TicketId: {ticketId}");
                itemDTO.EntryTime = DateTime.Now;
                var item = await _ticketRepo.FindByTicketId(ticketId);
                bool isSuccess = false;
                // if the poolid field is empty -> 1st visit of the day, update the record
                if (item.PoolId == null)
                {
                    itemDTO.Charges--;
                    item = _mapper.Map<Ticket>(itemDTO);
                    _logger.LogInfo($"{controllerName}: First Entry of the Day - TicketId: {ticketId}");
                    isSuccess = await _ticketRepo.Update(item);
                    if (isSuccess)
                    {
                        return Ok(itemDTO);
                    }
                    else
                    {
                        return InternalError($"{controllerName}: Update failed - TicketId: {ticketId}");
                    }
                }
                // if the pool ids are the same -> same pool doesn't lose charge
                if (item.PoolId == itemDTO.PoolId)
                {
                    _logger.LogInfo($"{controllerName}: Returning to the same pool - TicketId: {ticketId}");
                    item = _mapper.Map<Ticket>(itemDTO);
                }
                // if the poolids are different but there is more than one charge, lose 1 charge
                if (item.PoolId != itemDTO.PoolId && item.Charges > 0)
                {
                    _logger.LogInfo($"{controllerName}: Moving to a different pool - TicketId: {ticketId}");
                    itemDTO.Charges--;
                    item = _mapper.Map<Ticket>(itemDTO);
                }
                // if the pool ids are different and there is no charge send an error message
                if (item.PoolId != itemDTO.PoolId && item.Charges == 0)
                {
                    _logger.LogInfo($"{controllerName}: Invalid ticket - TicketId: {ticketId}");
                    return BadRequest("Ticket is not valid anymore, please buy a new one");
                }
                if (item.Charges > 10)
                {
                    item.Charges = 12;
                }
                isSuccess = await _ticketRepo.Create(item);
                if (isSuccess)
                {
                    _logger.LogInfo($"{controllerName}: Update successful - TicketId: {ticketId}");
                    return Ok(itemDTO);
                }
                else
                {
                    return InternalError($"{controllerName}: Update failed - TicketId: {ticketId}");
                }
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
