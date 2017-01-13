using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vigil.Domain.EventSourcing;
using Vigil.Domain.Messaging;

namespace Vigil.WebApi.Controllers
{
    [Route("api/[controller]")]
    public abstract class BaseController<TEntity> : Controller
        where TEntity : class, IEventSourced
    {
        private readonly ICommandQueue commandQueue;
        private readonly Func<VigilWebContext> contextFactory;

        protected ICommandQueue CommandQueue { get { return commandQueue; } }
        protected Func<VigilWebContext> ContextFactory { get { return contextFactory; } }

        public BaseController(ICommandQueue commandQueue, Func<VigilWebContext> contextFactory)
        {
            this.commandQueue = commandQueue;
            this.contextFactory = contextFactory;
        }

        [HttpGet]
        public OkObjectResult Get()
        {
            using (var context = ContextFactory())
            {
                var entities = context.Set<TEntity>().Where(en => en.DeletedOn == null).ToList();
                return Ok(new ReadOnlyCollection<TEntity>(entities));
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            using (var context = contextFactory())
            {
                var entity = context.Set<TEntity>().Find(id);
                if (entity == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(entity);
                }
            }
        }

    }
}
