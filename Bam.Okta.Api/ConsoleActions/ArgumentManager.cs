using Bam.Net;

namespace Bam.Okta.Api.ConsoleActions
{
    public class ArgumentManager : CommandLineTool
    {
        public static void AddArguments()
        {
            AddValidArgument("userId", false);
        }
    }
}