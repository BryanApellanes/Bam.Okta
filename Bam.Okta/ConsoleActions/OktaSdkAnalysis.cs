using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bam.Net;
using Bam.Net.CommandLine;

namespace Bam.Okta.ConsoleActions
{
    public class OktaSdkAnalysis: CommandLineTool
    {
        [ConsoleAction("analyzeBaseOktaClient")]
        public void AnalyzeOktaClient()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            Type oldType = oldAssembly.GetTypes().FirstOrDefault(t => $"{t.Namespace}.{t.Name}".Equals("Okta.Sdk.OktaClient"));
            Type newType = newAssembly.GetTypes().FirstOrDefault(t => $"{t.Namespace}.{t.Name}".Equals("Okta.Sdk.OktaClient"));
            TypeChangeDescriptor typeChangeDescriptor = new TypeChangeDescriptor(oldType, newType);
            PrintTypeChangeDescriptor(typeChangeDescriptor);
        }

        [ConsoleAction("analyzeClient")]
        public void AnalyzeClient()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");
            string clientName = GetArgument("client", "Please specify the name of the client to analyze");
            if (!clientName.StartsWith("Okta.Sdk."))
            {
                clientName = $"Okta.Sdk.{clientName}";
            }

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            Type oldType = oldAssembly.GetTypes().FirstOrDefault(t => $"{t.Namespace}.{t.Name}".Equals(clientName));
            Type newType = newAssembly.GetTypes().FirstOrDefault(t => $"{t.Namespace}.{t.Name}".Equals(clientName));
            TypeChangeDescriptor typeChangeDescriptor = new TypeChangeDescriptor(oldType, newType);
            PrintTypeChangeDescriptor(typeChangeDescriptor);
        }
        
        [ConsoleAction("listRemovedClientMethods")]
        public void ListRemovedOktaClientMethods()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            string modelResourceTypeName = "Okta.Sdk.OktaClient";
            BaseTypeChangeDescriptor resourceChangeDescriptor = new BaseTypeChangeDescriptor(modelResourceTypeName, oldAssembly, newAssembly);
            foreach (TypeChangeDescriptor typeChangeDescriptor in resourceChangeDescriptor.ChangedTypes)
            {
                if (typeChangeDescriptor.HasRemovedMethods)
                {
                    Message.PrintLine(typeChangeDescriptor.TypeName, ConsoleColor.DarkCyan);
                    PrintRemovedMethods(typeChangeDescriptor);
                }
            }
        }
        
        [ConsoleAction("listAddedClientMethods")]
        public void ListAddedOktaClientMethods()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            string modelResourceTypeName = "Okta.Sdk.OktaClient";
            BaseTypeChangeDescriptor resourceChangeDescriptor = new BaseTypeChangeDescriptor(modelResourceTypeName, oldAssembly, newAssembly);
            foreach (TypeChangeDescriptor typeChangeDescriptor in resourceChangeDescriptor.ChangedTypes)
            {
                if (typeChangeDescriptor.HasAddedMethods)
                {
                    Message.PrintLine(typeChangeDescriptor.TypeName, ConsoleColor.DarkCyan);
                    PrintAddedMethods(typeChangeDescriptor);
                }
            }
        }
        
        [ConsoleAction("listRemovedResourceMethods")]
        public void ListRemovedResourceMethods()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            string modelResourceTypeName = "Okta.Sdk.Resource";
            BaseTypeChangeDescriptor resourceChangeDescriptor = new BaseTypeChangeDescriptor(modelResourceTypeName, oldAssembly, newAssembly);
            foreach (TypeChangeDescriptor typeChangeDescriptor in resourceChangeDescriptor.ChangedTypes)
            {
                if (typeChangeDescriptor.HasRemovedMethods)
                {
                    Message.PrintLine(typeChangeDescriptor.TypeName, ConsoleColor.DarkCyan);
                    PrintRemovedMethods(typeChangeDescriptor);
                }
            }
        }
        
        [ConsoleAction("analyzeClients")]
        public void AnalyzeClients()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            string oktaClientBaseTypeName = "Okta.Sdk.OktaClient";
            BaseTypeChangeDescriptor oktaClientChangeDescriptor = new BaseTypeChangeDescriptor(oktaClientBaseTypeName, oldAssembly, newAssembly);
            // Removed clients
            PrintRemovedTypes(oktaClientChangeDescriptor);

            // Added clients
            PrintAddedTypes(oktaClientChangeDescriptor);
            
            // Changed clients
            PrintChangedTypes(oktaClientChangeDescriptor);
        }

        [ConsoleAction("analyzeModels")]
        public void AnalyzeModels()
        {
            string oldPath = GetArgument("old", "Please specify the path to the old assembly");
            string newPath = GetArgument("new", "Please specify the path to the new assembly");

            Assembly oldAssembly = Assembly.LoadFile(oldPath);
            Assembly newAssembly = Assembly.LoadFile(newPath);
            // show added models
            string modelResourceTypeName = "Okta.Sdk.Resource";
            BaseTypeChangeDescriptor resourceChangeDescriptor = new BaseTypeChangeDescriptor(modelResourceTypeName, oldAssembly, newAssembly);
            // Removed resources
            PrintRemovedTypes(resourceChangeDescriptor);

            // Added resources
            PrintAddedTypes(resourceChangeDescriptor);
            
            // Changed resources
            PrintChangedTypes(resourceChangeDescriptor);
        }

        private static void PrintChangedTypes(BaseTypeChangeDescriptor changeDescriptor)
        {
            if (changeDescriptor.HasChangedTypes)
            {
                foreach (TypeChangeDescriptor typeChangeDescriptor in changeDescriptor.ChangedTypes)
                {
                    PrintTypeChangeDescriptor(typeChangeDescriptor);
                }
            }
            else
            {
                Message.PrintLine("\tNo types were changed", ConsoleColor.Yellow);
            }
        }

        private static void PrintTypeChangeDescriptor(TypeChangeDescriptor typeChangeDescriptor)
        {
            Message.PrintLine(typeChangeDescriptor.TypeName, ConsoleColor.Green);
            PrintRemovedMethods(typeChangeDescriptor);
            PrintAddedMethods(typeChangeDescriptor);
            PrintRemovedProperties(typeChangeDescriptor);
            PrintAddedProperties(typeChangeDescriptor);
        }

        private static void PrintAddedMethods(TypeChangeDescriptor typeChangeDescriptor)
        {
            if (typeChangeDescriptor.HasAddedMethods)
            {
                Message.PrintLine("\t\tAdded methods:", ConsoleColor.Blue);
                Thread.Sleep(1);
                foreach (string method in typeChangeDescriptor.AddedMethods.Keys)
                {
                    Message.PrintLine("\t\t\t`{0}`", ConsoleColor.Blue, method);
                }
            }
            else
            {
                Message.PrintLine("\t\tNo added methods", ConsoleColor.Blue);
            }
        }

        private static void PrintRemovedMethods(TypeChangeDescriptor typeChangeDescriptor)
        {
            if (typeChangeDescriptor.HasRemovedMethods)
            {
                Message.PrintLine("\t\tRemoved methods:", ConsoleColor.DarkCyan);
                foreach (string method in typeChangeDescriptor.RemovedMethods.Keys)
                {
                    Message.PrintLine("\t\t\t`{0}`", ConsoleColor.DarkCyan, method);
                }
            }
            else
            {
                Message.PrintLine("\t\tNo removed methods", ConsoleColor.DarkCyan);
            }
        }

        private static void PrintAddedTypes(BaseTypeChangeDescriptor changeDescriptor)
        {
            Message.PrintLine("Added extenders of {0}:", ConsoleColor.DarkYellow, changeDescriptor.BaseTypeName);
            if (changeDescriptor.HasAddedTypes)
            {
                changeDescriptor.AddedTypes.Each(addedType =>
                    Message.PrintLine($"\t- `{ChangeDescriptor.GetTypeKey(addedType)}`", ConsoleColor.DarkCyan));
            }
            else
            {
                Message.PrintLine("\tNo types were added", ConsoleColor.Yellow);
            }
        }

        private static void PrintRemovedTypes(BaseTypeChangeDescriptor changeDescriptor)
        {
            Message.PrintLine("Removed extenders of {0}:", ConsoleColor.Yellow, changeDescriptor.BaseTypeName);
            if (changeDescriptor.HasRemovedTypes)
            {
                changeDescriptor.RemovedTypes.Each(removedType =>
                    Message.PrintLine($"\t{ChangeDescriptor.GetTypeKey(removedType)}", ConsoleColor.DarkYellow));
            }
            else
            {
                Message.PrintLine("\tNo clients were removed", ConsoleColor.Yellow);
            }
        }

        private static void PrintAddedProperties(TypeChangeDescriptor typeChangeDescriptor)
        {
            Message.PrintLine("Added properties for {0}", ConsoleColor.Yellow, typeChangeDescriptor.TypeName);
            if (typeChangeDescriptor.HasAddedProperties)
            {
                typeChangeDescriptor.AddedProperties.Each(prop=> Message.PrintLine($"\t{prop}", ConsoleColor.DarkYellow));
            }
            else
            {
                Message.PrintLine("\tNo properties were added");
            }
        }
        
        private static void PrintRemovedProperties(TypeChangeDescriptor typeChangeDescriptor)
        {
            Message.PrintLine("Removed properties for {0}", ConsoleColor.Yellow, typeChangeDescriptor.TypeName);
            if (typeChangeDescriptor.HasRemovedProperties)
            {
                typeChangeDescriptor.RemovedProperties.Each(prop=> Message.PrintLine($"\t{prop}", ConsoleColor.DarkYellow));
            }
            else
            {
                Message.PrintLine("\tNo properties were removed");
            }
        }
    }
}