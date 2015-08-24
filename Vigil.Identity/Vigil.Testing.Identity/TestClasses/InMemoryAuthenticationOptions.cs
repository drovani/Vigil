using System;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Vigil.Testing.Identity.TestClasses
{
    public class InMemoryAuthenticationOptions : AuthenticationOptions
    {
        public PathString CallbackPath { get; private set; }
        public string UserName { get; private set; }
        public Guid UserId { get; private set; }
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; private set; }

        public InMemoryAuthenticationOptions(Guid userId, string userName)
            : base("InMemory")
        {
            Description.Caption = "InMemory";
            CallbackPath = new PathString("/signin-inmemory");
            AuthenticationMode = AuthenticationMode.Passive;
            UserName = userName;
            UserId = userId;
        }
    }
}
