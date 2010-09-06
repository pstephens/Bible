#region Copyright Notice

/* Copyright 2009-2010 Peter Stephens

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Builder
{
    public static class Composition
    {
        private static readonly Lazy<AggregateCatalog> aggCatalog = new Lazy<AggregateCatalog>(
            () => new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly())), true);

        private static readonly Lazy<CompositionContainer> container = new Lazy<CompositionContainer>(
            () => new CompositionContainer(aggCatalog.Value, true));

        private static readonly object locker = new object();
        private static readonly HashSet<Assembly> registeredAssemblies = new HashSet<Assembly>();

        public static void RegisterAssembly(Assembly assembly)
        {
            lock (locker)
            {
                if (registeredAssemblies.Contains(assembly)) return;
                aggCatalog.Value.Catalogs.Add(new AssemblyCatalog(assembly));
                registeredAssemblies.Add(assembly);
            }
        }

        public static CompositionContainer Container
        {
            get { return container.Value; }
        }
    }
}