using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Vigil.Domain.Messaging;
using Vigil.Patrons;
using Vigil.Patrons.Commands;

namespace Vigil.WebApi.Controllers
{
    public class PatronController : BaseController<Patron>
    {
        public PatronController(ICommandQueue commandQueue, Func<VigilWebContext> contextFactory)
            : base(commandQueue, contextFactory) { }

        [HttpPost]
        public IActionResult Create([FromBody]CreatePatron command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                CommandQueue.Publish(command);
                return Accepted(Url.Action(nameof(Get), new { id = command.PatronId }));
            }
        }

        [HttpPut("{patronId:guid}")]
        public IActionResult UpdateHeader(Guid patronId, [FromBody]UpdatePatronHeader command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (patronId != command.PatronId)
            {
                ModelState.AddModelError(nameof(command.PatronId), "Patron Id does not match command target.");
                return BadRequest(ModelState);
            }

            using var context = ContextFactory();
            if (context.Patrons.Find(command.PatronId) != null)
            {
                CommandQueue.Publish(command);
                return Accepted(Url.Action(nameof(Get), new { id = command.PatronId }));
            }
            else
            {
                return NotFound(command.PatronId);
            }
        }

        [HttpDelete("{patronId:guid}")]
        public IActionResult Delete(Guid patronId)
        {
            using var context = ContextFactory();
            if (context.Patrons.Any(p => p.Id == patronId && p.DeletedOn == null))
            {
                CommandQueue.Publish(new DeletePatron(User.Identity.Name ?? "Anonymous User", DateTime.UtcNow) { PatronId = patronId });
                return Accepted();
            }
            else
            {
                return NotFound(patronId);
            }
        }
    }
}
