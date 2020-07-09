using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bam.Net;

namespace Bam.Okta
{
    public abstract class AssemblyChangeDescriptor : ChangeDescriptor
    {
        public Assembly OldAssembly { get; set; }
        public Assembly NewAssembly { get; set; }
        
        public static List<TypeChangeDescriptor> GetChangesForExtendersOf(string baseTypeName, Assembly oldAssembly, Assembly newAssembly)
        {
            //string baseTypeName = "Okta.Sdk.OktaClient";
            HashSet<Type> oldTypes = GetTypesWhoseBaseTypeIs(baseTypeName, oldAssembly).ToHashSet();
            HashSet<Type> newTypes = GetTypesWhoseBaseTypeIs(baseTypeName, newAssembly).ToHashSet();
            return oldTypes
                .Select(old => new TypeChangeDescriptor(old, newTypes.FirstOrDefault(t => TypeChangeDescriptor.AreEquivalent(old, t))))
                .ToList();
        }
    }
}