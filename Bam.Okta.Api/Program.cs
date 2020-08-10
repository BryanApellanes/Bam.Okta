using System;
using Bam.Net;
using Bam.Okta.Api.ConsoleActions;

namespace Bam.Okta.Api
{
    class Program: CommandLineTool
    {
        static void Main(string[] args)
        {
            ArgumentManager.AddArguments();
            ExecuteMain(args);
        }
    }
}