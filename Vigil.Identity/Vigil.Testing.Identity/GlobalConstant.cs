namespace Vigil.Testing.Identity
{
    public static class IdentityGlobalConstant
    {
        /// <summary><see cref="Microsoft.AspNet.Identity.Owin.OwinContextExtensions" /> prefix appended
        /// to the Type.AssemblyQualifiedName to form the key used in IOwinContext.Get&lt;T&gt; extension method.
        /// </summary>
        public static readonly string IdentityKeyPrefix = "AspNet.Identity.Owin:";
        /// <summary><see cref="System.Web.HttpContextBaseExtensions"/> key used in the HttpContextBase.Items
        /// indexer when retrieving the IDictionary&lt;string, object&gt; object.
        /// </summary>
        public static readonly string OwinEnvironmentKey = "owin.Environment";
    }
}
