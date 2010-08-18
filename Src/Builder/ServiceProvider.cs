using System;

namespace Builder
{
    public abstract class ServiceProvider<TRelated> : IServiceProvider<TRelated>
    {
        public T GetService<T>() where T : IService<TRelated>, new()
        {
            var service = new T();
            return service;
        }
    }
}