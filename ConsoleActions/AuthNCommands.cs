using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.CommandLine;
using Bam.Net.Testing;
using Okta.Auth.Sdk;
using Okta.Sdk;

namespace Okta.AuthN.ConsoleActions
{
    public partial class AuthNCommands: CommandLineTestInterface
    {
        [ConsoleAction("show-config", "Show the current configuration for the default OktaClient")]
        public async Task ShowConfiguration()
        {
            Message.PrintLine(new OktaClient().Configuration.ToJson(true));
        }

        [ConsoleAction("login", "Authenticate to Okta")]
        public async Task Login()
        {
            string userId = GetUserId();
            
        }
        
        [ConsoleAction("list-users", "List all users for the current configuration in ~/.okta/okta.yaml")]
        public async Task ListUsers()
        {
            OktaClient oktaClient = new OktaClient();
            IList<IUser> users = await oktaClient.Users.ListUsers().ToListAsync();

            foreach (IUser user in users)
            {
                PrintUser(user);
            }
        }

        [ConsoleAction("list-user-factors", "List factors for a user")]
        public async Task ListUserFactors()
        {
            string userId = GetUserId();
            OktaClient oktaClient = new OktaClient();
            IUser user = await oktaClient.Users.GetUserAsync(userId);
            PrintUser(user);
            AuthenticationClient authenticationClient = new AuthenticationClient();
            IList<IUserFactor> factors = await oktaClient.UserFactors.ListFactors(userId).ToListAsync();
            if (!factors.Any())
            {
                Message.PrintLine("No factors found for the specified user: {0}", ConsoleColor.DarkYellow, userId);
                return;
            }
            PrintFactors(factors);
        }

        [ConsoleAction("enroll-user-sms-factor", "Add a factor for a user")]
        public async Task EnrollFactor()
        {
            string userId = GetArgument("userId", "Please enter the id of the user whose factors should be retrieved");
            string testStateToken = 8.RandomLetters();
            EnrollSmsFactorOptions options = new EnrollSmsFactorOptions
            {
                PhoneExtension = "testPhoneExtension",
                PhoneNumber = "2062931640",
            };

            AuthenticationClient authenticationClient = new AuthenticationClient();
            IAuthenticationResponse response = await authenticationClient.EnrollFactorAsync(options);
            Message.Print(response);
        }
        
        [ConsoleAction("update-user-sms-factor", "Add a factor for a user")]
        public async Task UpdateFactor()
        {
            string userId = GetArgument("userId", "Please enter the id of the user whose factors should be retrieved");
            IList factorValues = (IList)Enum.GetValues(typeof(Factors));
            List<Factors> factors = new List<Factors>();
            foreach (Factors factor in factorValues)
            {
                factors.Add(factor);
            }
            string[] factorStrings = factors.ToArray()
                .Select<Factors, string>(x => x.ToString())
                .ToArray();

            Factors selectedFactor = SelectFrom(factors);
            //object options = GetEnrollFactorOptions(selectedFactor);

            AuthenticationClient authenticationClient = new AuthenticationClient();
            Okta.Sdk.Abstractions.HttpRequest request = new Okta.Sdk.Abstractions.HttpRequest()
            {
                Uri = "/api/v1/authn/factors?updatePhone=true",
                Payload = new EnrollSmsFactorOptions
                {
                    StateToken = "someRandomValue",
                    PhoneExtension = "PhoneExtensionValue",
                    PhoneNumber = "2062931640"
                },
            };
            AuthenticationResponse response = await authenticationClient.PostAsync<AuthenticationResponse>(request);
            Message.Print(response);
        }
    }
}