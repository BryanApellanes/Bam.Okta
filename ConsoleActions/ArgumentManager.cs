using Bam.Net;

namespace Okta.AuthN.ConsoleActions
{
    public class ArgumentManager : CommandLineTool
    {
        public static void AddArguments()
        {
            AddValidArgument("userId", false);
        }
    }
}