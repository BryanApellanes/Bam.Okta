using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bam.Net;

namespace Bam.Okta
{
    public class BaseTypeChangeDescriptor: AssemblyChangeDescriptor
    {
        public BaseTypeChangeDescriptor(string baseTypeName, Assembly oldAssembly, Assembly newAssembly)
        {
            OldAssembly = oldAssembly;
            NewAssembly = newAssembly;
            BaseTypeName = baseTypeName;
        }
        
        public string BaseTypeName
        {
            get;
        }

        public bool HasRemovedTypes => RemovedTypes.Any();
        public bool HasAddedTypes => AddedTypes.Any();
        public bool HasChangedTypes => ChangedTypes.Any();
        
        private List<TypeChangeDescriptor> _changedTypes;
        private readonly object _changedTypesLock = new object();
        public List<TypeChangeDescriptor> ChangedTypes
        {
            get
            {
                return _changedTypesLock.DoubleCheckLock(ref _changedTypes, () => GetChangesForExtendersOf(BaseTypeName, OldAssembly, NewAssembly));
            }
        }
        
        private List<Type> _removedTypes;
        private readonly object _removedTypesLock = new object();
        public List<Type> RemovedTypes
        {
            get
            {
                return _removedTypesLock.DoubleCheckLock(ref _removedTypes, () =>
                {
                    Dictionary<string, Type> oldTypes = GetDictionaryOfTypesWhoseBaseTypeIs(BaseTypeName, OldAssembly);
                    Dictionary<string, Type> newTypes = GetDictionaryOfTypesWhoseBaseTypeIs(BaseTypeName, NewAssembly);
                    List<Type> results = new List<Type>();
                    foreach (string oldTypeName in oldTypes.Keys)
                    {
                        if (!newTypes.ContainsKey(oldTypeName))
                        {
                            results.Add(oldTypes[oldTypeName]);
                        }
                    }

                    return results;
                });
            }
        } 
        
        private List<Type> _addedTypes;
        private readonly object _addedTypesLock = new object();
        public List<Type> AddedTypes
        {
            get
            {
                return _addedTypesLock.DoubleCheckLock(ref _addedTypes, () =>
                {
                    Dictionary<string, Type> oldTypes = GetDictionaryOfTypesWhoseBaseTypeIs(BaseTypeName, OldAssembly);
                    Dictionary<string, Type> newTypes = GetDictionaryOfTypesWhoseBaseTypeIs(BaseTypeName, NewAssembly);
                    List<Type> results = new List<Type>();
                    foreach (string newTypeName in newTypes.Keys)
                    {
                        if (!oldTypes.ContainsKey(newTypeName))
                        {
                            results.Add(newTypes[newTypeName]);
                        }
                    }

                    return results;
                });
            }
        }
    }
}