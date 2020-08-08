using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Net;
using Bam.Net.CommandLine;
using Okta.Auth.Sdk;
using Bam.Okta.Api;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace Bam.Okta.Api.ConsoleActions
{
    public partial class AuthNCommands
    {
        private OktaApi GetOktaApi()
        {
            return new OktaApi();
        }

        private OktaClientConfiguration GetOktaClientConfiguration()
        {
            return BamProfile.LoadJsonData<OktaClientConfiguration>("okta/okta-config.yaml");
        }
        
        private object GetEnrollFactorOptions(Factors factor)
        {
            Type[] types = typeof(AuthenticationClient)
                .Assembly
                .GetTypes()
                .Where(type => type.Name.Equals($"Enroll{factor}FactorOptions"))
                .ToArray();

            if (types.Length > 1)
            {
                Message.PrintLine("Multiple factor option types found: {0}", ConsoleColor.DarkMagenta,
                    string.Join("\r\n\t ", types.Select(t => t.FullName).ToArray()));
            }
            else if(types.Length == 0)
            {
                Message.PrintLine("No options found for specified factor: {0}", ConsoleColor.Magenta,
                    factor.ToString());
            }
            else
            {
                return types.First().Construct();
            }

            return null;
        }
        
        private void PrintUsers(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                PrintUser(user);
            }
        }
        
        private void PrintUser(IUser user)
        {
            Message.PrintLine("{0}: {1}", ConsoleColor.Cyan, user.Id, user?.Profile?.Email ?? "email not found");
        }

        private void PrintFactors(IEnumerable<IUserFactor> factors)
        {
            PrintFactors(factors.ToArray());
        }

        private void PrintFactors(params IUserFactor[] factors)
        {
            PrintFactors("No factors specified", factors);
        }
        
        private void PrintFactors(string noFactorsMessage, params IUserFactor[] factors)
        {
            if (factors == null || factors.Length == 0)
            {
                Message.PrintLine(noFactorsMessage, ConsoleColor.Yellow);
                return;
            }
            foreach (IUserFactor factor in factors)
            {
                PrintFactor(factor);
            }
        }
        
        private void PrintFactor(IUserFactor factor)
        {
            Message.PrintLine(factor.ToJson(true));
        }
        
        private static string GetUserId(string prompt = "Please enter the user id")
        {
            return GetArgument("userId", prompt);
        }

        private static string GetFactorId(string prompt = "Please enter the factor id")
        {
            return GetArgument("factorId", prompt);
        }

        private static string GetPassCode(string prompt = "Please enter the passcode")
        {
            return GetArgument("passCode", prompt);
        }
        
        private static void PrintProcessOutput(ProcessOutput output)
        {
            Message.PrintLine(output.StandardOutput);
            if (!string.IsNullOrEmpty(output.StandardError))
            {
                Message.PrintLine(output.StandardError, ConsoleColor.Magenta);
            }
        }
    }
}