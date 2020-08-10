using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.CoreServices;
using Okta.Auth.Sdk;
using Okta.Sdk;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;
using IOktaClient = Okta.Sdk.IOktaClient;
using Configuration = Okta.Sdk.Configuration;


namespace Bam.Okta.Api
{
    public class OktaApi
    {
        public OktaApi(ServiceRegistry serviceRegistry = null)
        {
            ServiceRegistry = serviceRegistry ?? new ServiceRegistry();
            AuthenticationClient = ServiceRegistry.Get<IAuthenticationClient>(AuthN);
            ManagementClient = ServiceRegistry.Get<IOktaClient>(Management);
        }
        
        public ServiceRegistry ServiceRegistry { get; set; }
        public IAuthenticationClient AuthenticationClient { get; set; }
        public IOktaClient ManagementClient { get; set; }

        public static OktaClientConfiguration GetConfiguration()
        {
            return BamProfile.LoadJsonData<OktaClientConfiguration>("okta/okta-config.yaml");
        }
        
        public static AuthenticationClient AuthN => new AuthenticationClient(GetConfiguration());
        public static OktaClient Management => new OktaClient(GetConfiguration().CopyAs<Configuration.OktaClientConfiguration>());

        public async Task<IAuthenticationResponse> AuthenticateAsync(string userName, string password)
        {
            return await AuthenticateAsync(AuthN, userName, password);
        }
        public async Task<IAuthenticationResponse> AuthenticateAsync(IAuthenticationClient client, string userName, string password)
        {
            return await client.AuthenticateAsync(new AuthenticateOptions
            {
                Username = userName,
                Password = password
            });
        }
    }
}