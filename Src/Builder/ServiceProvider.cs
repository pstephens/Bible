using System;
using System.Collections.Generic;

namespace Builder
{
    public abstract class ServiceProvider<TRelated> : IServiceProvider<TRelated> where TRelated : class
    {
        private readonly Dictionary<Type, IService<TRelated>> services =
            new Dictionary<Type, IService<TRelated>>();

        public T GetService<T>() where T : class, IService<TRelated>, new()
        {
            IService<TRelated> service;
            if (!services.TryGetValue(typeof(T), out service))
            {
                service = new T {Related = this as TRelated};
                if(service.Related == null)
                    throw new InvalidOperationException("ServiceProvider (Related) must implement TRelated.");
                services.Add(typeof (T), service);
            }

            return (T) service;
        }
    }
}