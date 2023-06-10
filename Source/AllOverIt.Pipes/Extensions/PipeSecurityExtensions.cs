using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AllOverIt.Pipes.Extensions
{
    public static class PipeSecurityExtensions
    {
        public static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, WellKnownSidType wellKnownSid, PipeAccessRights accessRight,
            AccessControlType accessControl)
        {
            // Get the identity of the user account to grant or deny access
            var identity = new SecurityIdentifier(wellKnownSid, null);

            return pipeSecurity.AddIdentityAccessRule(identity, accessRight, accessControl);
        }

        public static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, WellKnownSidType wellKnownSid, SecurityIdentifier domainSid,
            PipeAccessRights accessRight, AccessControlType accessControl)
        {
            // Get the identity of the user account to grant or deny access
            var identity = new SecurityIdentifier(wellKnownSid, domainSid);

            return pipeSecurity.AddIdentityAccessRule(identity, accessRight, accessControl);
        }

        private static PipeSecurity AddIdentityAccessRule(this PipeSecurity pipeSecurity, SecurityIdentifier identity, PipeAccessRights accessRight,
            AccessControlType accessControl)
        {
            // Create an access rule to grant or deny specific rights to the user
            var accessRule = new PipeAccessRule(identity, accessRight, accessControl);

            pipeSecurity.AddAccessRule(accessRule);

            return pipeSecurity;
        }
    }
}