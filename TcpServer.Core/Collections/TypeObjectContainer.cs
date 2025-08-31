using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Collections.Interfaces;
using TcpServer.Core.Interfaces;

namespace TcpServer.Core.Collections
{
    internal class TypeObjectContainer : IReadOnlyServices
    {
        private readonly Dictionary<Type, object> typeObjectDictionary;

        public TypeObjectContainer()
        {
            typeObjectDictionary = new Dictionary<Type, object>();
        }

        public void Clear()
        {
            typeObjectDictionary.Clear();
        }

        public object? Get(Type type)
        {
            return typeObjectDictionary.ContainsKey(type)
                ? typeObjectDictionary[type]
                : null;
        }

        public T? Get<T>() where T : class
        {
            return (T?)Get(typeof(T));
        }

        public void Set(object value)
        {
            if (value == this)
            {
                throw new ArgumentException("Cannot set the TypeObjectContainer itself as a value.", nameof(value));
            }

            typeObjectDictionary[value.GetType()] = value;
        }
        public void Set<T, TImplementation>(TImplementation value)
        {
            if ((object)value! == this)
            {
                throw new ArgumentException("Cannot set the TypeObjectContainer itself as a value.", nameof(value));
            }

            typeObjectDictionary[typeof(T)] = value!;
        }
    }
}
