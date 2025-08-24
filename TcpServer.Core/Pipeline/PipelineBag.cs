using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Pipeline.Interfaces;

namespace TcpServer.Core.Pipeline
{
    internal class PipelineBag : IPipelineBag
    {
        private readonly Dictionary<Type, object> objectsPipelineBag;

        public PipelineBag()
        {
            this.objectsPipelineBag = new Dictionary<Type, object>();
        }

        public void Clear()
        {
            this.objectsPipelineBag.Clear();
        }

        public object? Get(Type type)
        {
            return this.objectsPipelineBag.ContainsKey(type)
                ? this.objectsPipelineBag[type]
                : null;
        }

        public T? Get<T>() where T : class
        {
            return (T?)this.Get(typeof(T));
        }

        public void Set(object value)
        {
            if (value == this)
            {
                throw new ArgumentException("Cannot set the PipelineBag itself as a value.", nameof(value));
            }

            this.objectsPipelineBag[value.GetType()] = value;
        }
    }
}
