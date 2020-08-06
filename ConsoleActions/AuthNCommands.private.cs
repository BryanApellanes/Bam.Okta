using System;
using System.Collections.Generic;
using System.Linq;
using Bam.Net;
using Bam.Net.CommandLine;
using Okta.Auth.Sdk;
using Okta.Sdk;

namespace Okta.AuthN.ConsoleActions
{
    public partial class AuthNCommands
    {
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
            if (factors == null || factors.Length == 0)
            {
                Message.PrintLine("No factors specified", ConsoleColor.Yellow);
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
        
        private static string GetUserId()
        {
            return GetArgument("userId", "Please enter the id of the user whose factors should be retrieved");
        }
    }
}