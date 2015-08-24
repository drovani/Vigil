using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace Vigil.Testing.Identity.TestClasses
{
    // AuthenticationTypes, 
    using AuthenticateDelegate = Func<string[], Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>, object, Task>;

    public class InMemoryAuthenticationManager : IAuthenticationManager
    {
        // Code shamelessly cribbed from the Katana Project
        // https://katanaproject.codeplex.com/SourceControl/latest#src/Microsoft.Owin/Security/AuthenticationManager.cs

        internal AuthenticateDelegate AuthenticateDelegate { get; set; }

        private IPrincipal user;
        public ClaimsPrincipal User
        {
            get
            {
                if (user == null) return null;
                return user as ClaimsPrincipal ?? new ClaimsPrincipal(user);
            }
            set
            {
                user = value;
            }
        }

        private Tuple<string[], IDictionary<string, string>> challengeEntry = null;
        private Tuple<IPrincipal, IDictionary<string, string>> signInEntry = null;
        private Tuple<string[], IDictionary<string, string>> signOutEntry = null;

        public AuthenticationResponseChallenge AuthenticationResponseChallenge
        {
            get
            {
                if (challengeEntry == null)
                {
                    return null;
                }
                else
                {
                    return new AuthenticationResponseChallenge(challengeEntry.Item1, new AuthenticationProperties(challengeEntry.Item2));
                }
            }
            set
            {
                if (value == null)
                {
                    challengeEntry = null;
                }
                else
                {
                    challengeEntry = Tuple.Create(value.AuthenticationTypes, value.Properties.Dictionary);
                }
            }
        }
        public AuthenticationResponseGrant AuthenticationResponseGrant
        {
            get
            {
                if (signInEntry == null)
                {
                    return null;
                }
                else
                {
                    return new AuthenticationResponseGrant(signInEntry.Item1 as ClaimsPrincipal ?? new ClaimsPrincipal(signInEntry.Item1), new AuthenticationProperties(signInEntry.Item2));
                }
            }
            set
            {
                if (value == null)
                {
                    signInEntry = null;
                }
                else
                {
                    signInEntry = Tuple.Create((IPrincipal)value.Principal, value.Properties.Dictionary);
                }
            }
        }
        public AuthenticationResponseRevoke AuthenticationResponseRevoke
        {
            get
            {
                if (signOutEntry == null)
                {
                    return null;
                }
                else
                {
                    return new AuthenticationResponseRevoke(signOutEntry.Item1, new AuthenticationProperties(signOutEntry.Item2));
                }
            }
            set
            {
                if (value == null)
                {
                    signOutEntry = null;
                }
                else
                {
                    signOutEntry = Tuple.Create(value.AuthenticationTypes, value.Properties.Dictionary);
                }
            }
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
        {
            return GetAuthenticationTypes(_ => true);
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes(Func<AuthenticationDescription, bool> predicate)
        {
            var descriptions = new List<AuthenticationDescription>();
            GetAuthenticationTypes(raw =>
            {
                var description = new AuthenticationDescription(raw);
                if (predicate(description))
                {
                    descriptions.Add(description);
                }
            }).Wait();
            return descriptions;
        }

        public async Task<IEnumerable<AuthenticateResult>> AuthenticateAsync(string[] authenticationTypes)
        {
            var results = new List<AuthenticateResult>();
            await Authenticate(authenticationTypes, AuthenticateAsyncCallback, results);
            return results;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(string authenticationType)
        {
            return (await AuthenticateAsync(new string[] { authenticationType })).SingleOrDefault();
        }

        public void Challenge(params string[] authenticationTypes)
        {
            Challenge(new AuthenticationProperties(), authenticationTypes);
        }

        public void Challenge(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            AuthenticationResponseChallenge priorChallenge = AuthenticationResponseChallenge;
            if (priorChallenge == null)
            {
                AuthenticationResponseChallenge = new AuthenticationResponseChallenge(authenticationTypes, properties);
            }
            else
            {
                string[] mergedAuthTypes = priorChallenge.AuthenticationTypes.Concat(authenticationTypes).ToArray();
                if (properties != null && !Object.ReferenceEquals(properties.Dictionary, priorChallenge.Properties.Dictionary))
                {
                    foreach (var propertiesPair in properties.Dictionary)
                    {
                        priorChallenge.Properties.Dictionary[propertiesPair.Key] = propertiesPair.Value;
                    }
                }

                AuthenticationResponseChallenge = new AuthenticationResponseChallenge(mergedAuthTypes, priorChallenge.Properties);
            }
        }

        public void SignIn(params ClaimsIdentity[] identities)
        {
            SignIn(new AuthenticationProperties(), identities);
        }

        public void SignIn(AuthenticationProperties properties, params ClaimsIdentity[] identities)
        {
            AuthenticationResponseRevoke priorRevoke = AuthenticationResponseRevoke;
            if (priorRevoke != null)
            {
                string[] filteredSignOuts = priorRevoke.AuthenticationTypes
                    .Where(authType => !identities.Any(identity => identity.AuthenticationType.Equals(authType, StringComparison.Ordinal)))
                    .ToArray();
                if (filteredSignOuts.Length < priorRevoke.AuthenticationTypes.Length)
                {
                    if (filteredSignOuts.Length == 0)
                    {
                        AuthenticationResponseRevoke = null;
                    }
                    else
                    {
                        AuthenticationResponseRevoke = new AuthenticationResponseRevoke(filteredSignOuts);
                    }
                }
            }

            AuthenticationResponseGrant priorGrant = AuthenticationResponseGrant;
            if (priorGrant == null)
            {
                AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identities), properties);
            }
            else
            {
                ClaimsIdentity[] mergedIdentities = priorGrant.Principal.Identities.Concat(identities).ToArray();
                if (properties != null && !Object.ReferenceEquals(properties.Dictionary, priorGrant.Properties.Dictionary))
                {
                    foreach (var propertiesPair in properties.Dictionary)
                    {
                        priorGrant.Properties.Dictionary[propertiesPair.Key] = propertiesPair.Value;
                    }
                }
                AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(mergedIdentities), priorGrant.Properties);
            }

        }

        public void SignOut(params string[] authenticationTypes)
        {
            SignOut(new AuthenticationProperties(), authenticationTypes);
        }

        public void SignOut(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            AuthenticationResponseGrant priorGrant = AuthenticationResponseGrant;
            if (priorGrant != null)
            {
                ClaimsIdentity[] filtredIdentities = priorGrant.Principal.Identities
                    .Where(identity => !authenticationTypes.Contains(identity.AuthenticationType, StringComparer.Ordinal))
                    .ToArray();
                if (filtredIdentities.Length < priorGrant.Principal.Identities.Count())
                {
                    if (filtredIdentities.Length == 0)
                    {
                        AuthenticationResponseGrant = null;
                    }
                    else
                    {
                        AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(filtredIdentities), priorGrant.Properties);
                    }
                }
            }

            AuthenticationResponseRevoke priorRevoke = AuthenticationResponseRevoke;
            if (priorRevoke == null)
            {
                AuthenticationResponseRevoke = new AuthenticationResponseRevoke(authenticationTypes, properties);
            }
            else
            {
                if (properties != null && !Object.ReferenceEquals(properties.Dictionary, priorRevoke.Properties.Dictionary))
                {
                    foreach (var propertiesPair in properties.Dictionary)
                    {
                        priorRevoke.Properties.Dictionary[propertiesPair.Key] = propertiesPair.Value;
                    }
                }
                string[] mergedAuthTypes = priorRevoke.AuthenticationTypes.Concat(authenticationTypes).ToArray();
                AuthenticationResponseRevoke = new AuthenticationResponseRevoke(mergedAuthTypes, priorRevoke.Properties);
            }
        }

        private async Task Authenticate(string[] authenticationTypes, Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object> callback, object state)
        {
            AuthenticateDelegate authenticateDelegate = AuthenticateDelegate;
            if (authenticateDelegate != null)
            {
                await authenticateDelegate.Invoke(authenticationTypes, callback, state);
            }
        }
        private Task GetAuthenticationTypes(Action<IDictionary<string, object>> callback)
        {
            return Authenticate(null, (_, __, description, ___) => callback(description), null);
        }
        private static void AuthenticateAsyncCallback(IIdentity identity, IDictionary<string, string> properties, IDictionary<string, object> description, object state)
        {
            List<AuthenticateResult> list = (List<AuthenticateResult>)state;
            list.Add(new AuthenticateResult(identity, new AuthenticationProperties(properties), new AuthenticationDescription(description)));
        }
    }
}
