using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vigil.Domain.Messaging
{
    public interface ICommandQueue
    {
        void Publish<TCommand>(TCommand @event) where TCommand : ICommand;
    }
}
