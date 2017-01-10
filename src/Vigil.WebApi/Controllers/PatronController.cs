using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Vigil.Domain.Messaging;
using Vigil.Patrons;
using Vigil.Patrons.Commands;

namespace Vigil.WebApi.Controllers
{
    [Route("api/[controller]")]
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
                command.PatronId = Guid.NewGuid();
                CommandQueue.Publish(command);
                return Accepted(Url.Action(nameof(Get), new { id = command.PatronId }));
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateHeader(Guid id, [FromBody] UpdatePatronHeader command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var context = ContextFactory())
            {
                if (context.Patrons.Find(id) != null)
                {
                    command.PatronId = id;
                    CommandQueue.Publish(command);
                    return Accepted(Url.Action(nameof(Get), new { id = id }));
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
            using (var context = ContextFactory())
            {
                if (context.Patrons.Any(p => p.Id == id))
                {
                    CommandQueue.Publish(new DeletePatron(User.Identity.Name, DateTime.Now.ToUniversalTime()) { PatronId = id });
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
