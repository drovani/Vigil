using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Vigil.Domain.Messaging;
using Vigil.Patrons.Commands;

namespace Vigil.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PatronController : Controller
    {
        private readonly ICommandQueue commandQueue;
        private readonly Func<VigilWebContext> contextFactory;

        public PatronController(ICommandQueue commandQueue, Func<VigilWebContext> contextFactory)
        {
            this.commandQueue = commandQueue;
            this.contextFactory = contextFactory;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            using (var context = contextFactory())
            {
                var patron = context.Patrons.Find(id);
                if (patron == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(patron);
                }
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]CreatePatron command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                commandQueue.Publish(command);
                return Accepted(Url.Action(nameof(Get), new { id = command.PatronId }));
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateHeader(Guid id,
            [Bind(nameof(UpdatePatronHeader.DisplayName),
                  nameof(UpdatePatronHeader.IsAnonymous),
                  nameof(UpdatePatronHeader.PatronType))] UpdatePatronHeader command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return BadRequest(command);
            }
            using (var context = contextFactory())
            {
                if (context.Patrons.Find(id) == null)
                {
                    command.PatronId = id;
                    commandQueue.Publish(command);
                    return Accepted(Url.Action(nameof(Get), new { id = command.PatronId }));
                }
                else
                {
                    return NotFound(id);
                }
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            using (var context = contextFactory())
            {
                if (context.Patrons.Any(p => p.Id == id))
                {
                    commandQueue.Publish(new DeletePatron(User.Identity.Name, DateTime.Now) { PatronId = id });
                    return Accepted();
                }
                else
                {
                    return NotFound(id);
                }
            }
        }
    }
}
