using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bam.Net;
using Bam.Net.CommandLine;
using Bam.Net.Testing;
using Okta.Auth.Sdk;
using Bam.Okta.Api;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using FactorType = Okta.Sdk.FactorType;
using HttpRequest = Okta.Sdk.Abstractions.HttpRequest;

namespace Bam.Okta.Api.ConsoleActions
{
    public partial class AuthNCommands: CommandLineTestInterface
    {
        [ConsoleAction("Test")]
        public async Task Test()
        {
            ProcessInfo current = ProcessInfo.Current;
            ProcessStartInfo startInfo = current.ToStartInfo();
            Message.Print(current);
        }
        
        [ConsoleAction("show-config", "Show the current configuration for the default OktaClient")]
        public async Task ShowConfiguration()
        {
            Message.PrintLine(new OktaClient(GetOktaClientConfiguration()).Configuration.ToJson(true));
        }

        [ConsoleAction("full-sms-enroll-lifecycle", "Execute the entire sms factor enrollment api call sequence")]
        public async Task ExecuteFullSmsEnrollmentLifecycle()
        {
            ProcessInfo process = ProcessInfo.Current;
            ProcessOutput output = process.ReRun(process.EntryAssembly, "/list-users");
            PrintProcessOutput(output);

            string userId = Prompt("Enter the userId to enroll");
            string phoneNumber = GetArgument("phoneNumber", "Please enter the phone number to enroll");
            output = process.ReRun(process.EntryAssembly, "/enroll-sms-user-factor", $"/phoneNumber:{phoneNumber}");
            PrintProcessOutput(output);

            string passCode = Prompt($"Enter the passCode sent to {phoneNumber}");

            output = process.ReRun(process.EntryAssembly, "/activate-sms-factor", $"/passCode:{passCode}");
            PrintProcessOutput(output);

            output = process.ReRun(process.EntryAssembly, "/list-user-factors", $"/userId:{userId}");
            PrintProcessOutput(output);
        }

        [ConsoleAction("list-users", "List all users for the current configuration in ~/.okta/okta.yaml")]
        public async Task ListUsers()
        {
            OktaApi oktaApi = GetOktaApi();
            IOktaClient oktaClient = oktaApi.ManagementClient;
            IList<IUser> users = await oktaClient.Users.ListUsers().ToListAsync();

            foreach (IUser user in users)
            {
                PrintUser(user);
            }
        }
        
        [ConsoleAction("list-supported-factors", "List supported factors for a user")]
        public async Task ListSupportedFactors()
        {
            string userId = GetUserId();
            OktaApi oktaApi = GetOktaApi();
            IOktaClient oktaClient = oktaApi.ManagementClient;
            IList<IUserFactor> supportedFactors = await oktaClient.UserFactors.ListSupportedFactors(userId).ToListAsync();
            PrintFactors(supportedFactors.ToArray());
        }
        
        [ConsoleAction("enroll-sms-user-factor", "Enroll Sms factor for user")]
        public async Task EnrollSmsFactor()
        {
            string userId = GetUserId();
            IOktaClient oktaClient = GetOktaApi().ManagementClient;
            string phoneNumber = GetArgument("phoneNumber", "Please enter the sms phone number to enroll");
            UserFactor userFactor = new UserFactor
            {
                FactorType = FactorType.Sms,
                Provider = FactorProvider.Okta
            };
            
            userFactor.SetProperty("profile", new
            {
                phoneNumber = phoneNumber
            });

            IUserFactor userFactorResponse = await oktaClient.UserFactors.EnrollFactorAsync(userFactor, userId, true);
            Message.Print(userFactorResponse);
            Message.PrintLine("You should receive a passcode at {0}, save this for the activate step",
                ConsoleColor.Yellow, phoneNumber);
        }

        [ConsoleAction("activate-sms-factor", "Activate Sms factor for user")]
        public async Task ActivateSmsFactor()
        {
            string userId = GetUserId();
            string factorId = GetFactorId();
            IOktaClient oktaClient = GetOktaApi().ManagementClient;
            
            IUserFactor userFactor = await oktaClient.UserFactors.ActivateFactorAsync(new ActivateFactorRequest
            {
                PassCode = GetPassCode()
            }, userId, factorId);
            
            Message.Print(userFactor);
        }

        [ConsoleAction("list-enrolled-user-factors", "List enrolled factors for a given user")]
        public async Task ListUserFactors()
        {
            string userId = GetUserId();
            IOktaClient oktaClient = GetOktaApi().ManagementClient;
            IList<IUserFactor> userFactors = await oktaClient.UserFactors.ListFactors(userId).ToListAsync();
            PrintFactors($"No factors found for user {userId}", userFactors.ToArray());
        }
        
        [ConsoleAction("update-user-sms-factor", "Update sms factor for user")]
        public async Task UpdateUserFactor()
        {
            string userId = GetUserId();
            string factorId = GetFactorId();
            IOktaClient oktaClient = GetOktaApi().ManagementClient;
            await oktaClient.UserFactors.DeleteFactorAsync(userId, factorId);
        }

        [ConsoleAction("delete-user-factor", "Delete user factor")]
        public async Task DeleteUserFactor()
        {
            string userId = GetUserId();
            string factorId = GetFactorId();
            IOktaClient oktaClient = GetOktaApi().ManagementClient;
            await oktaClient.UserFactors.DeleteFactorAsync(userId, factorId);
        }
    }
}