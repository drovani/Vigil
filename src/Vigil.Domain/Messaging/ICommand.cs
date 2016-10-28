using System;

namespace Vigil.Domain.Messaging
{
    public interface ICommand
    {
        /// <summary>Gets the command identifier.
        /// </summary>
        Guid Id { get; }
    }
}
