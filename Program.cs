using System;
using Bam.Net;
using Okta.AuthN.ConsoleActions;

namespace Okta.AuthN
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