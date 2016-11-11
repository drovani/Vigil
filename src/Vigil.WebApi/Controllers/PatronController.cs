using System;
using Microsoft.AspNetCore.Mvc;
using Vigil.Patrons.Commands;
using Vigil.Domain.Messaging;
using System.Linq;

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
        public IActionResult Post([FromBody]CreatePatron command)
        {
            if (command == null)
            {
                return BadRequest();
            }
            else if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                commandQueue.Publish(command);
                return CreatedAtAction(nameof(Get), new { id = command.PatronId }, command);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id,
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
                if (context.Patrons.Any(p => p.Id == id))
                {
                    command.PatronId = id;
                    commandQueue.Publish(command);
                    return Ok();
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
                    commandQueue.Publish(new DeletePatron { PatronId = id });
                    return Ok();
                }
                else
                {
                    return NotFound(id);
                }
            }
        }
    }
}
