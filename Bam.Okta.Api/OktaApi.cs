using Bam.Net.CoreServices;
using Okta.Auth.Sdk;
using Okta.Sdk;

namespace Bam.Okta.Api
{
    public class OktaApi
    {
        public OktaApi(ServiceRegistry serviceRegistry = null)
        {
            ServiceRegistry = serviceRegistry ?? new ServiceRegistry();
            AuthenticationClient = serviceRegistry.Get<IAuthenticationClient>(new AuthenticationClient());
            ManagementClient = serviceRegistry.Get<IOktaClient>(new OktaClient());
        }
        
        public ServiceRegistry ServiceRegistry { get; set; }
        public IAuthenticationClient AuthenticationClient { get; set; }
        public IOktaClient ManagementClient { get; set; }
    }
}