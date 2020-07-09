using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bam.Net;

namespace Bam.Okta
{
    public class TypeChangeDescriptor : ChangeDescriptor
    {
        public TypeChangeDescriptor(Type oldType, Type newType)
        {
            if (oldType!= null && newType != null &&  !AreEquivalent(oldType, newType))
            {
                throw new InvalidOperationException($"Specified types are not equivalent: {GetTypeKey(oldType)} {GetTypeKey(newType)} ");
            }

            OldType = oldType;
            NewType = newType;
        }
        public string TypeName => GetTypeKey(OldType);
        public bool HasRemovedMethods => RemovedMethods.Any();
        public bool HasAddedMethods => AddedMethods.Any();
        public bool TypeWasRemoved => NewType == null;
        public bool HasAddedProperties => AddedProperties.Any();
        public bool HasRemovedProperties => RemovedProperties.Any();
        public Type OldType { get; }
        public Type NewType { get; }

        private Dictionary<string, MethodInfo> _oldMethods;
        public Dictionary<string, MethodInfo> OldMethods => _oldMethods ??= GetMethodsBySignature(OldType);

        private Dictionary<string, MethodInfo> _newMethods;
        public Dictionary<string, MethodInfo> NewMethods => _newMethods ??= GetMethodsBySignature(NewType);

        private Dictionary<string, MethodInfo> _addedMethods;
        private readonly  object _addedMethodLock = new object();
        public Dictionary<string, MethodInfo> AddedMethods
        {
            get
            {
                return _addedMethodLock.DoubleCheckLock(ref _addedMethods, () =>
                { 
                    Dictionary<string, MethodInfo> addedMethods = new Dictionary<string, MethodInfo>();
                    foreach (string key in NewMethods.Keys)
                    {
                        if (!OldMethods.ContainsKey(key))
                        {
                            addedMethods.Add(key, NewMethods[key]);
                        }
                    }
                    return addedMethods;
                });
            }
        }

        private Dictionary<string, MethodInfo> _removedMethods;
        private readonly object _removedMethodLock = new object();
        public Dictionary<string, MethodInfo> RemovedMethods
        {
            get
            {
                return _removedMethodLock.DoubleCheckLock(ref _removedMethods, () =>
                {
                    Dictionary<string, MethodInfo> removedMethods = new Dictionary<string, MethodInfo>();
                    foreach (string key in OldMethods.Keys)
                    {
                        if (!NewMethods.ContainsKey(key))
                        {
                            removedMethods.Add(key, OldMethods[key]);
                        }
                    }

                    return removedMethods;
                });
            }
        }

        private List<PropertyInfo> _oldProperties;
        private readonly object _oldPropertiesLock = new object();
        public List<PropertyInfo> OldProperties
        {
            get
            {
                return _oldPropertiesLock.DoubleCheckLock(ref _oldProperties, () => OldType.GetProperties().Where(t=> t.DeclaringType == OldType).ToList());
            }
        }

        private List<PropertyInfo> _newProperties;
        private readonly object _newPropertiesLock = new object();
        public List<PropertyInfo> NewProperties
        {
            get
            {
                return _newPropertiesLock.DoubleCheckLock(ref _newProperties, () => NewType == null ? new List<PropertyInfo>(): NewType.GetProperties().Where(t=> t.DeclaringType == NewType).ToList());
            }
        }

        public List<string> RemovedProperties
        {
            get
            {
                HashSet<string> oldProperties = OldProperties.Select(p => $"{p.PropertyType.FullName} {p.Name}").ToHashSet();
                HashSet<string> newProperties = NewProperties.Select(p => $"{p.PropertyType.FullName} {p.Name}").ToHashSet();
                return oldProperties.Except(newProperties).ToList();
            }
        }

        public List<string> AddedProperties
        {
            get
            {
                HashSet<string> oldProperties = OldProperties.Select(p => $"{p.PropertyType.FullName} {p.Name}").ToHashSet();
                HashSet<string> newProperties = NewProperties.Select(p => $"{p.PropertyType.FullName} {p.Name}").ToHashSet();
                return newProperties.Except(oldProperties).ToList();
            }
        }
                
        public static List<TypeChangeDescriptor> FromDictionary(Dictionary<Type, Type> oldToNewTypes)
        {
            List<TypeChangeDescriptor> results = new List<TypeChangeDescriptor>();
            foreach (Type oldType in oldToNewTypes.Keys)
            {
                results.Add(new TypeChangeDescriptor(oldType, oldToNewTypes[oldType]));
            }

            return results;
        }
        
        public static bool AreEquivalent(Type oldType, Type newType)
        {
            if(oldType == null && newType != null ||
                oldType != null && newType == null)
                {
                    return false;
                }
            return $"{oldType.Namespace}.{oldType.Name}".Equals($"{newType.Namespace}.{newType.Name}");
        }
        
        private Dictionary<string, MethodInfo> GetMethodsBySignature(Type type)
        {
            Dictionary<string, MethodInfo> results = new Dictionary<string, MethodInfo>();
            if (type != null)
            {
                foreach (MethodInfo methodInfo in type.GetMethods().Where(mi => mi.DeclaringType == type && !mi.Name.StartsWith("get_") && !mi.Name.StartsWith("set_")))
                {
                    string signature = GetMethodSignature(methodInfo);
                    if (!results.ContainsKey(signature))
                    {
                        results.Add(signature, methodInfo);
                    }
                }
            }

            return results;
        }
        
        private string GetMethodSignature(MethodInfo method)
        {
            return $"{method.Name}({string.Join(",", method.GetParameters().Select(GetNamedParameterList).ToArray())})";
        }

        private string GetNamedParameterList(ParameterInfo parameterInfo)
        {
            return $"{GetParameterTypeName(parameterInfo)} {parameterInfo.Name}";
        }

        private string GetParameterTypeName(ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.FullName.StartsWith("System"))
            {
                if (parameterInfo.ParameterType.IsGenericType)
                {
                    return parameterInfo.ParameterType.GenericTypeArguments.First().Name;
                }
                return parameterInfo.ParameterType.Name;
            }

            return parameterInfo.ParameterType.FullName;
        }
    }
}