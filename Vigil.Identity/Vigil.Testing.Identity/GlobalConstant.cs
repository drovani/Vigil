namespace Vigil.Testing.Identity
{
    internal static class GlobalConstant
    {
        /// <summary><see cref="Microsoft.AspNet.Identity.Owin.OwinContextExtensions" /> prefix appended
        /// to the Type.AssemblyQualifiedName to form the key used in IOwinContext.Get&lt;T&gt; extension method.
        /// </summary>
        public const string IdentityKeyPrefix = "AspNet.Identity.Owin:";
    }
}
