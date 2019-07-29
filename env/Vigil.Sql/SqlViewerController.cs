using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vigil.Patrons;
using Vigil.Patrons.Events;
using Vigil.Sql;

namespace Vigil.WebApi.Controllers
{
    [Route("sql")]
    public class SqlViewerController : ControllerBase
    {
        private readonly Func<SqlMessageDbContext> _dbFactory;

        public SqlViewerController(Func<SqlMessageDbContext> contextFactory)
        {
            _dbFactory = contextFactory;
        }

        [HttpGet("commands")]
        public OkObjectResult GetAllCommands()
        {
            List<Command> cmds;
            using (var context = _dbFactory())
            {
                cmds = context.Commands.OrderByDescending(c => c.GeneratedOn).ToList();
            }
            return Ok(cmds);
        }

        [HttpGet("events")]
        public OkObjectResult GetAllEvents()
        {
            List<Event> events;
            using (var context = _dbFactory())
            {
                events = context.Events.OrderByDescending(c => c.GeneratedOn).ToList();
            }
            return Ok(events);
        }

        [HttpGet("rehydrate/{patronId:guid}")]
        public IActionResult RehydratePatron(Guid patronId)
        {
            List<Event> events;
            using (var context = _dbFactory())
            {
                events = context.Events.Where(ev => ev.SerializedEvent.Contains("PatronId")).ToList();
            }
            List<PatronEvent> patronEvents = new List<PatronEvent>();
            foreach (var ev in events)
            {
                Type type = Type.GetType(ev.EventType, true);
                var obj = JsonConvert.DeserializeObject(ev.SerializedEvent, type);
                var pEvent = obj as PatronEvent;
                patronEvents.Add(pEvent);
            }

            var forPatron = patronEvents.Where(pc => pc.PatronId == patronId);

            if (forPatron.Any())
            {
                return Ok(new Patron(patronId, forPatron));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
