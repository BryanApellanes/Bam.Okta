using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bam.Net;

namespace Bam.Okta
{
    public abstract class ChangeDescriptor
    {
        protected static Dictionary<string, Type> GetDictionaryOfTypesWhoseBaseTypeIs(string baseTypeFullName, Assembly assembly)
        {
            return assembly.GetTypes().Where(type =>
                type.BaseType != null &&
                !string.IsNullOrEmpty(type.BaseType.FullName) &&
                type.BaseType.FullName.Equals(baseTypeFullName)).ToDictionary(type => $"{type.Namespace}.{type.Name}");
        }
        
        protected static List<Type> GetTypesWhoseBaseTypeIs(string baseTypeFullName, Assembly assembly)
        {
            List<Type> types = assembly.GetTypes().Where(type =>
                type.BaseType != null &&
                !string.IsNullOrEmpty(type.BaseType.FullName) &&
                type.BaseType.FullName.Equals(baseTypeFullName)).ToList();
            return types;
        }
        
        public static string GetTypeKey(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            return $"{type.Namespace}.{type.Name}";
        }
    }
}