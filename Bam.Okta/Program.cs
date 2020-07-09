using System;
using Bam.Net;

namespace Bam.Okta
{
    class Program : CommandLineTool
    {
        static Program()
        {
            AddValidArgument("old", "for /analyze, the path to the old assembly");
            AddValidArgument("new", "for /analyze, the path to the new assembly");
            AddValidArgument("client", "The name of the client to analyze");
        }
        
        static void Main(string[] args)
        {
            ExecuteMain(args);
        }
    }
}
